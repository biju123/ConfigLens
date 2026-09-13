using ConfigLens.Application.Validation.Rules;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;
using FluentAssertions;
using Xunit;
using static ConfigLens.Application.Tests.Validation.RuleEvaluatorTestHelpers;

namespace ConfigLens.Application.Tests.Validation.Rules;

public class RequiredExistsRuleEvaluatorTests
{
    private readonly RequiredExistsRuleEvaluator _sut = new();

    [Fact]
    public void Passes_when_the_item_exists()
    {
        var rule = Rule(RuleType.RequiredExists, "Database.RetryCount");
        var tree = TreeWith("Database", new ConfigItem("RetryCount", "3", false));

        _sut.Evaluate(rule, Context(tree)).Status.Should().Be(FindingStatus.Pass);
    }

    [Fact]
    public void Fails_when_the_item_is_missing()
    {
        var rule = Rule(RuleType.RequiredExists, "Database.RetryCount");
        var tree = TreeWith("Database", new ConfigItem("ConnectionString", "x", true));

        _sut.Evaluate(rule, Context(tree)).Status.Should().Be(FindingStatus.Fail);
    }

    [Fact]
    public void NotChecked_when_no_configuration_was_found()
    {
        var rule = Rule(RuleType.RequiredExists, "Database.RetryCount");

        _sut.Evaluate(rule, Context(null)).Status.Should().Be(FindingStatus.NotChecked);
    }
}
