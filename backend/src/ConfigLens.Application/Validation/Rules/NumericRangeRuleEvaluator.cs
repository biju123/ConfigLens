using System.Globalization;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;

namespace ConfigLens.Application.Validation.Rules;

public sealed class NumericRangeRuleEvaluator : RuleEvaluatorBase
{
    public override RuleType RuleType => RuleType.NumericRange;

    public override Finding Evaluate(ValidationRule rule, RuleEvaluationContext context)
    {
        var min = double.Parse(rule.Parameters.GetValueOrDefault("min", "0"), CultureInfo.InvariantCulture);
        var max = double.Parse(rule.Parameters.GetValueOrDefault("max", "0"), CultureInfo.InvariantCulture);
        var expected = $"{rule.TargetPath} is between {min} and {max}";

        if (context.Tree is null)
        {
            return Build(rule, context, FindingStatus.NotChecked, expected, "No configuration was found to check.");
        }

        var item = ConfigTreePathResolver.Resolve(context.Tree, rule.TargetPath);
        if (item is null || item.Value is null)
        {
            return Build(rule, context, FindingStatus.NotApplicable, expected, $"{rule.TargetPath} is not configured.");
        }

        if (!double.TryParse(item.Value, NumberStyles.Number, CultureInfo.InvariantCulture, out var numericValue))
        {
            return Build(rule, context, FindingStatus.Error, expected, $"{rule.TargetPath} value '{context.DisplayValue(item)}' is not numeric.");
        }

        var actual = $"{rule.TargetPath} is {numericValue}";
        return numericValue >= min && numericValue <= max
            ? Build(rule, context, FindingStatus.Pass, expected, actual)
            : Build(rule, context, FindingStatus.Fail, expected, actual);
    }
}
