using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Api.Tests;

public class DependencyAccessibilityScanControllerTests(ConfigLensApiFactory factory) : IClassFixture<ConfigLensApiFactory>
{
    [Fact]
    public async Task Scan_returns_checks_covering_multiple_statuses()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/scans/dependency-accessibility", new
        {
            application = "AssessorApi",
            environment = "Production",
            @namespace = "assessor-prod"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("checks").GetArrayLength().Should().Be(6);
    }
}
