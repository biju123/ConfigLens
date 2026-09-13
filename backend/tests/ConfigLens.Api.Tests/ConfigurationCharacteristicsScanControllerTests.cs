using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Api.Tests;

public class ConfigurationCharacteristicsScanControllerTests(ConfigLensApiFactory factory) : IClassFixture<ConfigLensApiFactory>
{
    [Fact]
    public async Task Scan_returns_findings_and_a_summary_with_failures_for_the_riverbend_scenario()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/scans/configuration-characteristics", new
        {
            application = "AssessorApi",
            environment = "Production",
            tenant = "RiverbendCounty",
            sessionYear = 2026,
            ruleSetId = "default"
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("summary").GetProperty("fail").GetInt32().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Scan_returns_400_invalid_input_for_an_unknown_rule_set()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/scans/configuration-characteristics", new
        {
            application = "AssessorApi",
            environment = "Production",
            tenant = "ContosoCounty",
            sessionYear = 2026,
            ruleSetId = "nonexistent"
        });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("invalid-input");
    }
}
