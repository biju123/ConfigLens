using ConfigLens.Application.Validation.Rules;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;
using FluentAssertions;
using Xunit;
using static ConfigLens.Application.Tests.Validation.RuleEvaluatorTestHelpers;

namespace ConfigLens.Application.Tests.Validation.Rules;

public class NotEmptyRuleEvaluatorTests
{
    private readonly NotEmptyRuleEvaluator _sut = new();

    [Fact]
    public void Fails_when_the_value_is_empty()
    {
        var rule = Rule(RuleType.NotEmpty, "ServiceBus.Queue");
        var tree = TreeWith("ServiceBus", new ConfigItem("Queue", "", false));

        _sut.Evaluate(rule, Context(tree)).Status.Should().Be(FindingStatus.Fail);
    }

    [Fact]
    public void Passes_when_the_value_is_non_empty()
    {
        var rule = Rule(RuleType.NotEmpty, "ServiceBus.Queue");
        var tree = TreeWith("ServiceBus", new ConfigItem("Queue", "assessor-events", false));

        _sut.Evaluate(rule, Context(tree)).Status.Should().Be(FindingStatus.Pass);
    }

    [Fact]
    public void NotApplicable_when_the_item_does_not_exist()
    {
        var rule = Rule(RuleType.NotEmpty, "ServiceBus.Queue");
        var tree = TreeWith("Database", new ConfigItem("RetryCount", "3", false));

        _sut.Evaluate(rule, Context(tree)).Status.Should().Be(FindingStatus.NotApplicable);
    }
}
