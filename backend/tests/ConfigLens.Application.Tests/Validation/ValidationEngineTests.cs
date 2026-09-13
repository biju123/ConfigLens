using ConfigLens.Application.Validation;
using ConfigLens.Application.Validation.Rules;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;
using FluentAssertions;
using Xunit;
using static ConfigLens.Application.Tests.Validation.RuleEvaluatorTestHelpers;

namespace ConfigLens.Application.Tests.Validation;

public class ValidationEngineTests
{
    private static readonly IRuleEvaluator[] AllEvaluators =
    [
        new RequiredExistsRuleEvaluator(), new NotEmptyRuleEvaluator(), new FormatMatchRuleEvaluator(),
        new AllowedValuesRuleEvaluator(), new NumericRangeRuleEvaluator(), new SessionYearExistsRuleEvaluator(),
        new CrossApplicationConsistencyRuleEvaluator(), new CrossEnvironmentConsistencyRuleEvaluator()
    ];

    [Fact]
    public void Evaluate_dispatches_each_rule_to_its_registered_evaluator()
    {
        var sut = new ValidationEngine(AllEvaluators);
        var ruleSet = new RuleSet("test", "Test", [
            Rule(RuleType.RequiredExists, "Database.RetryCount"),
            Rule(RuleType.NotEmpty, "ServiceBus.Queue")
        ]);
        var tree = TreeWith("Database", new ConfigItem("RetryCount", "3", false));

        var findings = sut.Evaluate(ruleSet, Context(tree));

        findings.Should().HaveCount(2);
        findings[0].Status.Should().Be(FindingStatus.Pass);
        findings[1].Status.Should().Be(FindingStatus.NotApplicable);
    }

    [Fact]
    public void Evaluate_throws_when_no_evaluator_is_registered_for_a_rule_type()
    {
        var sut = new ValidationEngine([new RequiredExistsRuleEvaluator()]);
        var ruleSet = new RuleSet("test", "Test", [Rule(RuleType.NotEmpty, "ServiceBus.Queue")]);

        var act = () => sut.Evaluate(ruleSet, Context(null));

        act.Should().Throw<InvalidOperationException>();
    }
}
