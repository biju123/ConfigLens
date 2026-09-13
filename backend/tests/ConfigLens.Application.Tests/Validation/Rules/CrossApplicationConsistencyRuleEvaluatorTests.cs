using ConfigLens.Application.Tests.Fakes;
using ConfigLens.Application.Validation.Rules;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;
using FluentAssertions;
using Xunit;
using static ConfigLens.Application.Tests.Validation.RuleEvaluatorTestHelpers;

namespace ConfigLens.Application.Tests.Validation.Rules;

public class CrossApplicationConsistencyRuleEvaluatorTests
{
    private readonly CrossApplicationConsistencyRuleEvaluator _sut = new();
    private static readonly Dictionary<string, string> Parameters = new() { ["compareApplication"] = "AssessorPortal" };

    [Fact]
    public void Warns_when_values_differ_between_applications()
    {
        var client = new FakeApplicationConfigurationClient()
            .With("AssessorPortal", "Production", "ContosoCounty", 2026, TreeWith("Database", new ConfigItem("CommandTimeout", "60", false)));

        var rule = Rule(RuleType.CrossApplicationConsistency, "Database.CommandTimeout", Parameters);
        var tree = TreeWith("Database", new ConfigItem("CommandTimeout", "30", false));

        _sut.Evaluate(rule, Context(tree, client: client)).Status.Should().Be(FindingStatus.Warning);
    }

    [Fact]
    public void Passes_when_values_match_between_applications()
    {
        var client = new FakeApplicationConfigurationClient()
            .With("AssessorPortal", "Production", "ContosoCounty", 2026, TreeWith("Database", new ConfigItem("CommandTimeout", "30", false)));

        var rule = Rule(RuleType.CrossApplicationConsistency, "Database.CommandTimeout", Parameters);
        var tree = TreeWith("Database", new ConfigItem("CommandTimeout", "30", false));

        _sut.Evaluate(rule, Context(tree, client: client)).Status.Should().Be(FindingStatus.Pass);
    }

    [Fact]
    public void NotApplicable_when_the_other_application_has_no_configuration()
    {
        var rule = Rule(RuleType.CrossApplicationConsistency, "Database.CommandTimeout", Parameters);
        var tree = TreeWith("Database", new ConfigItem("CommandTimeout", "30", false));

        _sut.Evaluate(rule, Context(tree)).Status.Should().Be(FindingStatus.NotApplicable);
    }
}
