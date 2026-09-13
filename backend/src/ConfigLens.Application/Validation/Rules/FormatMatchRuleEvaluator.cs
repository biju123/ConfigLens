using System.Text.RegularExpressions;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;

namespace ConfigLens.Application.Validation.Rules;

public sealed class FormatMatchRuleEvaluator : RuleEvaluatorBase
{
    public override RuleType RuleType => RuleType.FormatMatch;

    public override Finding Evaluate(ValidationRule rule, RuleEvaluationContext context)
    {
        var pattern = rule.Parameters.GetValueOrDefault("pattern", "");
        var expected = $"{rule.TargetPath} matches pattern {pattern}";

        if (context.Tree is null)
        {
            return Build(rule, context, FindingStatus.NotChecked, expected, "No configuration was found to check.");
        }

        var item = ConfigTreePathResolver.Resolve(context.Tree, rule.TargetPath);
        if (item is null || string.IsNullOrEmpty(item.Value))
        {
            return Build(rule, context, FindingStatus.NotApplicable, expected, $"{rule.TargetPath} is not configured.");
        }

        if (string.IsNullOrEmpty(pattern))
        {
            return Build(rule, context, FindingStatus.Error, expected, "Rule is missing a 'pattern' parameter.");
        }

        var matches = Regex.IsMatch(item.Value, pattern);
        var actual = $"{rule.TargetPath} is '{context.DisplayValue(item)}'";
        return matches
            ? Build(rule, context, FindingStatus.Pass, expected, actual)
            : Build(rule, context, FindingStatus.Fail, expected, actual);
    }
}
