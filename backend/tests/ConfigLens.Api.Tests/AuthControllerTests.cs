using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Api.Tests;

public class AuthControllerTests(ConfigLensApiFactory factory) : IClassFixture<ConfigLensApiFactory>
{
    [Fact]
    public async Task Login_returns_a_token_for_valid_credentials()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new { username = "user", password = "password" });

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("\"token\"");
    }

    [Fact]
    public async Task Login_returns_401_with_authentication_failure_type_for_invalid_credentials()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new { username = "user", password = "wrong" });

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("authentication-failure");
    }

    [Fact]
    public async Task Login_returns_400_invalid_input_for_missing_fields()
    {
        using var client = factory.CreateClient();

        var response = await client.PostAsJsonAsync("/api/auth/login", new { username = "", password = "" });

        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("invalid-input");
    }

    [Fact]
    public async Task A_request_without_a_token_is_rejected_with_401()
    {
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/reference-data");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
