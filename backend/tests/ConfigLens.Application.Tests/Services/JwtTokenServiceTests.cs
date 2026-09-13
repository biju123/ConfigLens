using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using ConfigLens.Application.Services;
using ConfigLens.Application.Tests.Fakes;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ConfigLens.Application.Tests.Services;

public class JwtTokenServiceTests
{
    private static JwtTokenService BuildSut(FakeClock clock)
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Jwt:SigningKey"] = "a-test-signing-key-that-is-long-enough-for-hmac-sha256" })
            .Build();
        return new JwtTokenService(config, clock);
    }

    [Fact]
    public void IssueToken_produces_a_token_that_expires_eight_hours_later()
    {
        var clock = new FakeClock(new DateTime(2026, 9, 12, 10, 0, 0, DateTimeKind.Utc));
        var (token, expiresAtUtc) = BuildSut(clock).IssueToken("user");

        token.Should().NotBeNullOrEmpty();
        expiresAtUtc.Should().Be(new DateTime(2026, 9, 12, 18, 0, 0, DateTimeKind.Utc));
    }

    [Fact]
    public void IssueToken_embeds_the_username_claim()
    {
        var clock = new FakeClock(DateTime.UtcNow);
        var (token, _) = BuildSut(clock).IssueToken("user");

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        jwt.Claims.Should().Contain(c => c.Type == ClaimTypes.Name && c.Value == "user");
    }

    [Fact]
    public void IssueToken_throws_when_signing_key_is_not_configured()
    {
        var config = new ConfigurationBuilder().Build();
        var sut = new JwtTokenService(config, new FakeClock(DateTime.UtcNow));

        var act = () => sut.IssueToken("user");

        act.Should().Throw<InvalidOperationException>();
    }
}
