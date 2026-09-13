using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;

namespace ConfigLens.Application.Validation.Rules;

public sealed class SessionYearExistsRuleEvaluator : RuleEvaluatorBase
{
    public override RuleType RuleType => RuleType.SessionYearExists;

    public override Finding Evaluate(ValidationRule rule, RuleEvaluationContext context)
    {
        var expected = $"Configuration exists for session year {context.SessionYear}";
        return context.Tree is null
            ? Build(rule, context, FindingStatus.Fail, expected, $"No configuration was found for session year {context.SessionYear}.")
            : Build(rule, context, FindingStatus.Pass, expected, $"Configuration exists for session year {context.SessionYear}.");
    }
}
