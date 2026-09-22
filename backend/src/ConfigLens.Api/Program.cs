using System.Text;
using System.Text.Json.Serialization;
using ConfigLens.Api.Filters;
using ConfigLens.Api.Middleware;
using ConfigLens.Api.Serialization;
using ConfigLens.Application.Auth;
using ConfigLens.Application.Comparison;
using ConfigLens.Application.Scans.Aks;
using ConfigLens.Application.Scans.AppConfig;
using ConfigLens.Application.Scans.Characteristics;
using ConfigLens.Application.Scans.Dependency;
using ConfigLens.Application.Services;
using ConfigLens.Application.Validation;
using ConfigLens.Application.Validation.Rules;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Reference;
using ConfigLens.Domain.Scan;
using ConfigLens.Domain.Scan.Aks;
using ConfigLens.Domain.Scan.Dependency;
using ConfigLens.Infrastructure.ApplicationApis;
using ConfigLens.Infrastructure.Azure;
using ConfigLens.Infrastructure.Kubernetes;
using ConfigLens.SampleData;
using Azure.Core;
using Azure.Identity;
using Azure.ResourceManager;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Map our documented CONFIGLENS_* env vars (see .env.example) onto the
// Section:Key configuration paths the app reads. ASP.NET Core's own
// environment-variable provider only understands double-underscore names
// (Auth__Username); using a single CONFIGLENS_ prefix instead avoids
// colliding with unrelated containers/tools sharing the same environment.
var envOverrides = new Dictionary<string, string?>();
void MapEnvVar(string envVarName, string configKey)
{
    var value = Environment.GetEnvironmentVariable(envVarName);
    if (!string.IsNullOrEmpty(value))
    {
        envOverrides[configKey] = value;
    }
}
MapEnvVar("CONFIGLENS_AUTH_USERNAME", "Auth:Username");
MapEnvVar("CONFIGLENS_AUTH_PASSWORD", "Auth:Password");
MapEnvVar("CONFIGLENS_JWT_SIGNING_KEY", "Jwt:SigningKey");
MapEnvVar("CONFIGLENS_FRONTEND_ORIGIN", "Cors:FrontendOrigin");
if (envOverrides.Count > 0)
{
    builder.Configuration.AddInMemoryCollection(envOverrides);
}

// --- Sample data / Infrastructure (Domain ports -> Infrastructure adapters) ---
builder.Services.AddSingleton<ISampleDataProvider, SampleDataProvider>();
builder.Services.AddSingleton<IApplicationConfigurationClient, SampleDataApplicationConfigurationClient>();
builder.Services.AddSingleton<IRuleSetProvider, SampleDataRuleSetProvider>();
builder.Services.AddSingleton<IDependencyAccessibilityChecker, SampleDataDependencyAccessibilityChecker>();
builder.Services.AddSingleton<IReferenceDataProvider, SampleDataReferenceDataProvider>();

// --- AKS Deployment Scan: real Azure Resource Manager / AKS API server integration.
// DefaultAzureCredential resolves via az-cli login, a managed identity, or the
// standard AZURE_* environment variables - never a credential baked into source
// or the Docker image (CLAUDE.md sections 19-20). ConfigLensApiFactory (tests)
// overrides these two registrations with the sample-data-backed adapters above
// so tests never need live Azure/AKS access.
builder.Services.AddSingleton<TokenCredential>(new DefaultAzureCredential());
builder.Services.AddSingleton(sp => new ArmClient(sp.GetRequiredService<TokenCredential>()));
builder.Services.AddSingleton<AksClusterConnector>();
builder.Services.AddSingleton<IAksClusterDirectory, AzureAksClusterDirectory>();
builder.Services.AddSingleton<IKubernetesInventoryReader, AzureKubernetesInventoryReader>();

// --- Cross-cutting Application services ---
builder.Services.AddSingleton<IClock, SystemClock>();
builder.Services.AddSingleton<IScanIdGenerator, ScanIdGenerator>();
builder.Services.AddSingleton<IScanRepository, InMemoryScanRepository>();
builder.Services.AddSingleton<IMaskingService, MaskingService>();
builder.Services.AddSingleton<ICredentialValidator, HardcodedCredentialValidator>();
builder.Services.AddSingleton<IJwtTokenService, JwtTokenService>();

// --- Validation engine: one IRuleEvaluator per RuleType, resolved as IEnumerable<IRuleEvaluator> ---
builder.Services.AddSingleton<IRuleEvaluator, RequiredExistsRuleEvaluator>();
builder.Services.AddSingleton<IRuleEvaluator, NotEmptyRuleEvaluator>();
builder.Services.AddSingleton<IRuleEvaluator, FormatMatchRuleEvaluator>();
builder.Services.AddSingleton<IRuleEvaluator, AllowedValuesRuleEvaluator>();
builder.Services.AddSingleton<IRuleEvaluator, NumericRangeRuleEvaluator>();
builder.Services.AddSingleton<IRuleEvaluator, SessionYearExistsRuleEvaluator>();
builder.Services.AddSingleton<IRuleEvaluator, CrossApplicationConsistencyRuleEvaluator>();
builder.Services.AddSingleton<IRuleEvaluator, CrossEnvironmentConsistencyRuleEvaluator>();
builder.Services.AddSingleton<IValidationEngine, ValidationEngine>();

// --- Scan services (one per independent category) ---
builder.Services.AddScoped<AksDeploymentScanService>();
builder.Services.AddScoped<AppConfigurationScanService>();
builder.Services.AddScoped<CharacteristicsScanService>();
builder.Services.AddScoped<DependencyAccessibilityScanService>();
builder.Services.AddScoped<IComparisonService, ComparisonService>();

// --- FluentValidation validators, registered explicitly (one per request DTO) ---
builder.Services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
builder.Services.AddScoped<IValidator<AksScanRequest>, AksScanRequestValidator>();
builder.Services.AddScoped<IValidator<AppConfigScanRequest>, AppConfigScanRequestValidator>();
builder.Services.AddScoped<IValidator<CharacteristicsScanRequest>, CharacteristicsScanRequestValidator>();
builder.Services.AddScoped<IValidator<DependencyScanRequest>, DependencyScanRequestValidator>();
builder.Services.AddScoped<IValidator<CompareScansRequest>, CompareScansRequestValidator>();

builder.Services.AddControllers(options => options.Filters.Add<ValidateRequestFilter>())
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.Converters.Add(new ScanIdJsonConverter());
    });

// Without this, MVC treats non-nullable reference-type request properties as
// implicitly [Required] and short-circuits with its own ModelState-based 400
// before ValidateRequestFilter runs - bypassing FluentValidation entirely and
// producing a differently-shaped error envelope. FluentValidation is meant to
// be the single source of truth for request validation (CLAUDE.md section 10).
builder.Services.Configure<ApiBehaviorOptions>(options =>
    options.SuppressModelStateInvalidFilter = true);

builder.Services.AddExceptionHandler<ConfigLensExceptionHandler>();
builder.Services.AddProblemDetails();

// Swagger UI does not expose a bearer-token "Authorize" button here - obtain
// a token from POST /api/auth/login and pass it as an Authorization header
// via curl or an HTTP client for the protected endpoints.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Read lazily (not into a variable captured before Build()) so this
        // reflects the fully-composed configuration - including sources a
        // host like WebApplicationFactory injects only once Build() runs.
        var jwtSigningKey = builder.Configuration["Jwt:SigningKey"];
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = "ConfigLens",
            ValidateAudience = true,
            ValidAudience = "ConfigLens",
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSigningKey ?? string.Empty))
        };

        // Ensures 401/403 come back as a ProblemDetails body so the frontend
        // can distinguish authentication vs authorization failure (CLAUDE.md section 23).
        options.Events = new JwtBearerEvents
        {
            OnChallenge = async context =>
            {
                context.HandleResponse();
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status401Unauthorized,
                    Type = "authentication-failure",
                    Title = "Authentication is required, or the supplied token is invalid or expired.",
                    Instance = context.Request.Path
                });
            },
            OnForbidden = async context =>
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                context.Response.ContentType = "application/problem+json";
                await context.Response.WriteAsJsonAsync(new ProblemDetails
                {
                    Status = StatusCodes.Status403Forbidden,
                    Type = "authorization-failure",
                    Title = "You do not have permission to perform this action.",
                    Instance = context.Request.Path
                });
            }
        };
    });
builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        var frontendOrigin = builder.Configuration["Cors:FrontendOrigin"];
        if (!string.IsNullOrEmpty(frontendOrigin))
        {
            policy.WithOrigins(frontendOrigin).AllowAnyHeader().AllowAnyMethod();
        }
    });
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseSwagger();
app.UseSwaggerUI();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/health", () => Results.Ok(new { status = "healthy" })).AllowAnonymous();

app.MapControllers();

app.Run();

public partial class Program;
