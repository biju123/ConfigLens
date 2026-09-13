using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;

namespace ConfigLens.Application.Validation.Rules;

public sealed class NotEmptyRuleEvaluator : RuleEvaluatorBase
{
    public override RuleType RuleType => RuleType.NotEmpty;

    public override Finding Evaluate(ValidationRule rule, RuleEvaluationContext context)
    {
        if (context.Tree is null)
        {
            return Build(rule, context, FindingStatus.NotChecked, $"{rule.TargetPath} is not empty", "No configuration was found to check.");
        }

        var item = ConfigTreePathResolver.Resolve(context.Tree, rule.TargetPath);
        if (item is null)
        {
            return Build(rule, context, FindingStatus.NotApplicable, $"{rule.TargetPath} is not empty", $"{rule.TargetPath} is not configured.");
        }

        return string.IsNullOrWhiteSpace(item.Value)
            ? Build(rule, context, FindingStatus.Fail, $"{rule.TargetPath} is not empty", $"{rule.TargetPath} is empty.")
            : Build(rule, context, FindingStatus.Pass, $"{rule.TargetPath} is not empty", $"{rule.TargetPath} has a value.");
    }
}
