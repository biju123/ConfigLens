using ConfigLens.Application.Validation.Rules;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;
using FluentAssertions;
using Xunit;
using static ConfigLens.Application.Tests.Validation.RuleEvaluatorTestHelpers;

namespace ConfigLens.Application.Tests.Validation.Rules;

public class FormatMatchRuleEvaluatorTests
{
    private readonly FormatMatchRuleEvaluator _sut = new();
    private static readonly Dictionary<string, string> Parameters = new() { ["pattern"] = "^sb-[a-z0-9-]+$" };

    [Fact]
    public void Passes_when_the_value_matches_the_pattern()
    {
        var rule = Rule(RuleType.FormatMatch, "ServiceBus.Namespace", Parameters);
        var tree = TreeWith("ServiceBus", new ConfigItem("Namespace", "sb-assessor-prod", false));

        _sut.Evaluate(rule, Context(tree)).Status.Should().Be(FindingStatus.Pass);
    }

    [Fact]
    public void Fails_when_the_value_does_not_match_the_pattern()
    {
        var rule = Rule(RuleType.FormatMatch, "ServiceBus.Namespace", Parameters);
        var tree = TreeWith("ServiceBus", new ConfigItem("Namespace", "ServiceBus-Prod-01", false));

        _sut.Evaluate(rule, Context(tree)).Status.Should().Be(FindingStatus.Fail);
    }

    [Fact]
    public void NotApplicable_when_the_item_is_missing()
    {
        var rule = Rule(RuleType.FormatMatch, "ServiceBus.Namespace", Parameters);
        var tree = TreeWith("Database", new ConfigItem("RetryCount", "3", false));

        _sut.Evaluate(rule, Context(tree)).Status.Should().Be(FindingStatus.NotApplicable);
    }
}
