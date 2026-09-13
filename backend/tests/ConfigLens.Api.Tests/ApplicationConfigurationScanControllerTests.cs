using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Api.Tests;

public class ApplicationConfigurationScanControllerTests(ConfigLensApiFactory factory) : IClassFixture<ConfigLensApiFactory>
{
    [Fact]
    public async Task Scan_masks_sensitive_values_in_the_response_body()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/scans/application-configuration", new
        {
            application = "AssessorApi",
            environment = "Production",
            tenant = "ContosoCounty",
            sessionYear = 2026
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var raw = await response.Content.ReadAsStringAsync();
        raw.Should().NotContain("Password=");
        raw.Should().Contain("********");

        var body = JsonSerializer.Deserialize<JsonElement>(raw);
        body.GetProperty("tree").GetProperty("rootName").GetString().Should().Be("AssessorApi");
    }

    [Fact]
    public async Task Scan_returns_an_empty_tree_for_an_unpublished_session_year()
    {
        var token = await factory.GetAuthTokenAsync();
        using var client = factory.CreateAuthenticatedClient(token);

        var response = await client.PostAsJsonAsync("/api/scans/application-configuration", new
        {
            application = "AssessorApi",
            environment = "Production",
            tenant = "ContosoCounty",
            sessionYear = 2024
        });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadFromJsonAsync<JsonElement>();
        body.GetProperty("tree").GetProperty("sections").GetArrayLength().Should().Be(0);
    }
}
