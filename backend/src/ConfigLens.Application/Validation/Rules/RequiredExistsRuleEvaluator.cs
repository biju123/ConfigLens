using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;

namespace ConfigLens.Application.Validation.Rules;

public sealed class RequiredExistsRuleEvaluator : RuleEvaluatorBase
{
    public override RuleType RuleType => RuleType.RequiredExists;

    public override Finding Evaluate(ValidationRule rule, RuleEvaluationContext context)
    {
        if (context.Tree is null)
        {
            return Build(rule, context, FindingStatus.NotChecked, $"{rule.TargetPath} is configured", "No configuration was found to check.");
        }

        var item = ConfigTreePathResolver.Resolve(context.Tree, rule.TargetPath);
        return item is null
            ? Build(rule, context, FindingStatus.Fail, $"{rule.TargetPath} is configured", $"{rule.TargetPath} is not configured.")
            : Build(rule, context, FindingStatus.Pass, $"{rule.TargetPath} is configured", $"{rule.TargetPath} is configured.");
    }
}
