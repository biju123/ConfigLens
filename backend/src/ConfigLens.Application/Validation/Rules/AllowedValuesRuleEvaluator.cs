using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;

namespace ConfigLens.Application.Validation.Rules;

public sealed class AllowedValuesRuleEvaluator : RuleEvaluatorBase
{
    public override RuleType RuleType => RuleType.AllowedValues;

    public override Finding Evaluate(ValidationRule rule, RuleEvaluationContext context)
    {
        var allowedValues = rule.Parameters.GetValueOrDefault("allowedValues", "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var expected = $"{rule.TargetPath} is one of [{string.Join(", ", allowedValues)}]";

        if (context.Tree is null)
        {
            return Build(rule, context, FindingStatus.NotChecked, expected, "No configuration was found to check.");
        }

        var item = ConfigTreePathResolver.Resolve(context.Tree, rule.TargetPath);
        if (item is null || item.Value is null)
        {
            return Build(rule, context, FindingStatus.NotApplicable, expected, $"{rule.TargetPath} is not configured.");
        }

        var actual = $"{rule.TargetPath} is '{context.DisplayValue(item)}'";
        var isAllowed = allowedValues.Contains(item.Value, StringComparer.OrdinalIgnoreCase);
        return isAllowed
            ? Build(rule, context, FindingStatus.Pass, expected, actual)
            : Build(rule, context, FindingStatus.Fail, expected, actual);
    }
}
