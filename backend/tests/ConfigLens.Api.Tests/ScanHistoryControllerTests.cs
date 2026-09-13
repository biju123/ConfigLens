using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Api.Tests;

public class ScanHistoryControllerTests(ConfigLensApiFactory factory) : IClassFixture<ConfigLensApiFactory>
{
    [Fact]
    public async Task GetById_returns_a_previously_completed_scan()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var scanResponse = await client.PostAsJsonAsync("/api/scans/dependency-accessibility", new
        {
            application = "AssessorApi",
            environment = "Staging",
            @namespace = "assessor-staging"
        });
        var scanId = (await scanResponse.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("metadata").GetProperty("scanId").GetString();

        var response = await client.GetAsync($"/api/scans/{scanId}");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_returns_404_for_an_unknown_scan_id()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/scans/SCAN-20200101-99999");

        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("not-found");
    }

    [Fact]
    public async Task GetById_returns_400_invalid_input_for_a_malformed_scan_id()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var response = await client.GetAsync("/api/scans/not-a-scan-id");

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}
