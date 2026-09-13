using ConfigLens.Application.Validation.Rules;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;
using FluentAssertions;
using Xunit;
using static ConfigLens.Application.Tests.Validation.RuleEvaluatorTestHelpers;

namespace ConfigLens.Application.Tests.Validation.Rules;

public class AllowedValuesRuleEvaluatorTests
{
    private readonly AllowedValuesRuleEvaluator _sut = new();
    private static readonly Dictionary<string, string> Parameters = new() { ["allowedValues"] = "AzureSql,SqlServer" };

    [Fact]
    public void Passes_when_the_value_is_allowed()
    {
        var rule = Rule(RuleType.AllowedValues, "Database.Provider", Parameters);
        var tree = TreeWith("Database", new ConfigItem("Provider", "AzureSql", false));

        _sut.Evaluate(rule, Context(tree)).Status.Should().Be(FindingStatus.Pass);
    }

    [Fact]
    public void Fails_when_the_value_is_not_allowed()
    {
        var rule = Rule(RuleType.AllowedValues, "Database.Provider", Parameters);
        var tree = TreeWith("Database", new ConfigItem("Provider", "PostgreSql", false));

        _sut.Evaluate(rule, Context(tree)).Status.Should().Be(FindingStatus.Fail);
    }
}
