using ConfigLens.Application.Validation.Rules;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;
using FluentAssertions;
using Xunit;
using static ConfigLens.Application.Tests.Validation.RuleEvaluatorTestHelpers;

namespace ConfigLens.Application.Tests.Validation.Rules;

public class SessionYearExistsRuleEvaluatorTests
{
    private readonly SessionYearExistsRuleEvaluator _sut = new();

    [Fact]
    public void Passes_when_configuration_exists()
    {
        var rule = Rule(RuleType.SessionYearExists, "$root");
        var tree = TreeWith("Database", new ConfigItem("RetryCount", "3", false));

        _sut.Evaluate(rule, Context(tree)).Status.Should().Be(FindingStatus.Pass);
    }

    [Fact]
    public void Fails_when_no_configuration_exists_for_the_session_year()
    {
        var rule = Rule(RuleType.SessionYearExists, "$root");

        _sut.Evaluate(rule, Context(null)).Status.Should().Be(FindingStatus.Fail);
    }
}
