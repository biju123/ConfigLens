using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Api.Tests;

public class AksDeploymentScanControllerTests(ConfigLensApiFactory factory) : IClassFixture<ConfigLensApiFactory>
{
    [Fact]
    public async Task Scan_returns_resources_for_a_valid_request()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/scans/aks-deployment", new
        {
            subscriptionId = "sub-assessor-prod-01",
            clusterName = "aks-assessor-prod-eastus",
            environment = "Production",
            tenant = "ContosoCounty",
            @namespace = "assessor-prod"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("resources").GetArrayLength().Should().BeGreaterThan(0);
        body.GetProperty("metadata").GetProperty("scanId").GetString().Should().MatchRegex(@"^SCAN-\d{8}-\d{5}$");
    }

    [Fact]
    public async Task Scan_returns_400_invalid_input_for_missing_required_fields()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/scans/aks-deployment", new { });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("invalid-input");
    }

    [Fact]
    public async Task Scan_without_a_token_returns_401()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/scans/aks-deployment", new { });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
