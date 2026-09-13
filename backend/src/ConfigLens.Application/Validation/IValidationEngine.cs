using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;

namespace ConfigLens.Application.Validation;

public interface IValidationEngine
{
    IReadOnlyList<Finding> Evaluate(RuleSet ruleSet, RuleEvaluationContext context);
}

/// <summary>Dispatches each rule to the IRuleEvaluator registered for its RuleType (strategy pattern - no single conditional chain to extend as rule types grow).</summary>
public sealed class ValidationEngine(IEnumerable<IRuleEvaluator> evaluators) : IValidationEngine
{
    private readonly IReadOnlyDictionary<RuleType, IRuleEvaluator> _evaluatorsByType =
        evaluators.ToDictionary(e => e.RuleType);

    public IReadOnlyList<Finding> Evaluate(RuleSet ruleSet, RuleEvaluationContext context) =>
        ruleSet.Rules.Select(rule => EvaluateRule(rule, context)).ToList();

    private Finding EvaluateRule(ValidationRule rule, RuleEvaluationContext context)
    {
        if (!_evaluatorsByType.TryGetValue(rule.RuleType, out var evaluator))
        {
            throw new InvalidOperationException($"No IRuleEvaluator is registered for rule type '{rule.RuleType}'.");
        }

        return evaluator.Evaluate(rule, context);
    }
}
