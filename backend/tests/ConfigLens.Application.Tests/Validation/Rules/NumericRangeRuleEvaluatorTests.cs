using ConfigLens.Application.Validation.Rules;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;
using FluentAssertions;
using Xunit;
using static ConfigLens.Application.Tests.Validation.RuleEvaluatorTestHelpers;

namespace ConfigLens.Application.Tests.Validation.Rules;

public class NumericRangeRuleEvaluatorTests
{
    private readonly NumericRangeRuleEvaluator _sut = new();
    private static readonly Dictionary<string, string> Parameters = new() { ["min"] = "5", ["max"] = "120" };

    [Fact]
    public void Passes_when_within_range()
    {
        var rule = Rule(RuleType.NumericRange, "Database.CommandTimeout", Parameters);
        var tree = TreeWith("Database", new ConfigItem("CommandTimeout", "30", false));

        _sut.Evaluate(rule, Context(tree)).Status.Should().Be(FindingStatus.Pass);
    }

    [Fact]
    public void Fails_when_out_of_range()
    {
        var rule = Rule(RuleType.NumericRange, "Database.CommandTimeout", Parameters);
        var tree = TreeWith("Database", new ConfigItem("CommandTimeout", "999", false));

        _sut.Evaluate(rule, Context(tree)).Status.Should().Be(FindingStatus.Fail);
    }

    [Fact]
    public void Errors_when_the_value_is_not_numeric()
    {
        var rule = Rule(RuleType.NumericRange, "Database.CommandTimeout", Parameters);
        var tree = TreeWith("Database", new ConfigItem("CommandTimeout", "not-a-number", false));

        _sut.Evaluate(rule, Context(tree)).Status.Should().Be(FindingStatus.Error);
    }
}
