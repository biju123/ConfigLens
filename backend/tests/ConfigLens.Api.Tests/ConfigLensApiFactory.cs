using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ConfigLens.Api.Tests;

/// <summary>Supplies deterministic Auth/Jwt/Cors configuration for tests instead of relying on appsettings.Development.json.</summary>
public sealed class ConfigLensApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureAppConfiguration((_, config) =>
        {
            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:Username"] = "user",
                ["Auth:Password"] = "password",
                ["Jwt:SigningKey"] = "test-only-signing-key-that-is-long-enough-for-hmac-sha256",
                ["Cors:FrontendOrigin"] = "http://localhost:5173"
            });
        });
    }

    public async Task<string> GetAuthTokenAsync()
    {
        using var client = CreateClient();
        var response = await client.PostAsJsonAsync("/api/auth/login", new { username = "user", password = "password" });
        response.EnsureSuccessStatusCode();
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("token").GetString()!;
    }

    public HttpClient CreateAuthenticatedClient(string token)
    {
        var client = CreateClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        return client;
    }
}
