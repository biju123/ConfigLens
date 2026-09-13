using ConfigLens.Application.Services;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace ConfigLens.Application.Tests.Services;

public class HardcodedCredentialValidatorTests
{
    private static HardcodedCredentialValidator BuildSut(string? username = "user", string? password = "password")
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:Username"] = username,
                ["Auth:Password"] = password
            })
            .Build();
        return new HardcodedCredentialValidator(config);
    }

    [Fact]
    public void Validate_returns_true_for_matching_credentials() =>
        BuildSut().Validate("user", "password").Should().BeTrue();

    [Fact]
    public void Validate_returns_false_for_wrong_password() =>
        BuildSut().Validate("user", "wrong").Should().BeFalse();

    [Fact]
    public void Validate_returns_false_for_wrong_username() =>
        BuildSut().Validate("someone-else", "password").Should().BeFalse();

    [Fact]
    public void Validate_returns_false_when_credentials_are_not_configured() =>
        BuildSut(username: null, password: null).Validate("user", "password").Should().BeFalse();
}
