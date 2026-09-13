using ConfigLens.Application.Tests.Fakes;
using ConfigLens.Application.Validation.Rules;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;
using FluentAssertions;
using Xunit;
using static ConfigLens.Application.Tests.Validation.RuleEvaluatorTestHelpers;

namespace ConfigLens.Application.Tests.Validation.Rules;

public class CrossEnvironmentConsistencyRuleEvaluatorTests
{
    private readonly CrossEnvironmentConsistencyRuleEvaluator _sut = new();
    private static readonly Dictionary<string, string> Parameters = new() { ["compareEnvironment"] = "Staging" };

    [Fact]
    public void Passes_when_values_match_between_environments()
    {
        var client = new FakeApplicationConfigurationClient()
            .With("AssessorApi", "Staging", "ContosoCounty", 2026, TreeWith("ServiceBus", new ConfigItem("Queue", "assessor-events", false)));

        var rule = Rule(RuleType.CrossEnvironmentConsistency, "ServiceBus.Queue", Parameters);
        var tree = TreeWith("ServiceBus", new ConfigItem("Queue", "assessor-events", false));

        _sut.Evaluate(rule, Context(tree, client: client)).Status.Should().Be(FindingStatus.Pass);
    }

    [Fact]
    public void Warns_when_values_differ_between_environments()
    {
        var client = new FakeApplicationConfigurationClient()
            .With("AssessorApi", "Staging", "ContosoCounty", 2026, TreeWith("ServiceBus", new ConfigItem("Queue", "assessor-events-2025", false)));

        var rule = Rule(RuleType.CrossEnvironmentConsistency, "ServiceBus.Queue", Parameters);
        var tree = TreeWith("ServiceBus", new ConfigItem("Queue", "assessor-events", false));

        _sut.Evaluate(rule, Context(tree, client: client)).Status.Should().Be(FindingStatus.Warning);
    }
}
