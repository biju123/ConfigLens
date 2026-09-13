using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Api.Tests;

public class ComparisonControllerTests(ConfigLensApiFactory factory) : IClassFixture<ConfigLensApiFactory>
{
    private async Task<string> RunAksScanAsync(HttpClient client, string tenant, string ns)
    {
        var response = await client.PostAsJsonAsync("/api/scans/aks-deployment", new
        {
            subscriptionId = "sub-assessor-prod-01",
            clusterName = "aks-assessor-prod-eastus",
            environment = "Production",
            tenant,
            @namespace = ns
        });
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        return body.GetProperty("metadata").GetProperty("scanId").GetString()!;
    }

    [Fact]
    public async Task Compare_returns_a_category_specific_diff_for_two_aks_scans()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var current = await RunAksScanAsync(client, "ContosoCounty", "assessor-prod");
        var baseline = await RunAksScanAsync(client, "RiverbendCounty", "assessor-prod-riverbend");

        var response = await client.PostAsJsonAsync("/api/comparisons", new { currentScanId = current, baselineScanId = baseline });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("category").GetString().Should().Be("AksDeployment");
        body.GetProperty("result").TryGetProperty("addedResources", out _).Should().BeTrue();
    }

    [Fact]
    public async Task Compare_returns_400_category_mismatch_for_scans_of_different_categories()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var aksScan = await RunAksScanAsync(client, "ContosoCounty", "assessor-prod");
        var depResponse = await client.PostAsJsonAsync("/api/scans/dependency-accessibility", new
        {
            application = "AssessorApi",
            environment = "Production",
            @namespace = "assessor-prod"
        });
        var depBody = await depResponse.Content.ReadFromJsonAsync<JsonElement>();
        var depScanId = depBody.GetProperty("metadata").GetProperty("scanId").GetString();

        var response = await client.PostAsJsonAsync("/api/comparisons", new { currentScanId = aksScan, baselineScanId = depScanId });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("category-mismatch");
    }

    [Fact]
    public async Task Compare_returns_404_when_a_scan_id_does_not_exist()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var current = await RunAksScanAsync(client, "ContosoCounty", "assessor-prod");

        var response = await client.PostAsJsonAsync("/api/comparisons", new { currentScanId = current, baselineScanId = "SCAN-20200101-99999" });

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
