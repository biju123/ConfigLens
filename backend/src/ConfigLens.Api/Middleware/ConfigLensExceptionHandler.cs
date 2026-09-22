using ConfigLens.Application.Comparison;
using ConfigLens.Application.Validation;
using ConfigLens.Domain.Comparison;
using ConfigLens.Domain.Scan.Aks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ConfigLens.Api.Middleware;

/// <summary>
/// Maps domain/application exceptions to a ProblemDetails envelope with a
/// stable `type` per CLAUDE.md section 23's error categories. Dependency-
/// inaccessible/timeout/rule-fail outcomes are NOT exceptions - those are
/// legitimate scan results returned as 200 OK with a status field in the
/// body; only malformed requests, auth problems and genuine faults land here.
/// </summary>
public sealed class ConfigLensExceptionHandler(ILogger<ConfigLensExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var (statusCode, type, title) = exception switch
        {
            ScanNotFoundException => (StatusCodes.Status404NotFound, "not-found", "Scan not found."),
            RuleSetNotFoundException => (StatusCodes.Status400BadRequest, "invalid-input", "Rule set not found."),
            CategoryMismatchException => (StatusCodes.Status400BadRequest, "category-mismatch", "The two scans do not belong to the same category."),
            AksClusterNotFoundException => (StatusCodes.Status404NotFound, "not-found", "AKS subscription or cluster not found."),
            AksConnectivityException => (StatusCodes.Status502BadGateway, "infrastructure-failure", "Unable to reach Azure or the AKS cluster."),
            AksTimeoutException => (StatusCodes.Status504GatewayTimeout, "timeout", "The request to Azure or the AKS cluster timed out."),
            FormatException => (StatusCodes.Status400BadRequest, "invalid-input", "The request was not in a valid format."),
            _ => (StatusCodes.Status500InternalServerError, "application-failure", "An unexpected error occurred.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception while processing {Path}", httpContext.Request.Path);
        }

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Type = type,
            Title = title,
            Detail = statusCode == StatusCodes.Status500InternalServerError ? null : exception.Message,
            Instance = httpContext.Request.Path
        }, cancellationToken);

        return true;
    }
}
