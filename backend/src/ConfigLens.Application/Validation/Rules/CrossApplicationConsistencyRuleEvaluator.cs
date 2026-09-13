using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;

namespace ConfigLens.Application.Validation.Rules;

public sealed class CrossApplicationConsistencyRuleEvaluator : RuleEvaluatorBase
{
    public override RuleType RuleType => RuleType.CrossApplicationConsistency;

    public override Finding Evaluate(ValidationRule rule, RuleEvaluationContext context)
    {
        var compareApplication = rule.Parameters.GetValueOrDefault("compareApplication", "");
        var expected = $"{rule.TargetPath} is consistent between {context.Application} and {compareApplication}";

        if (context.Tree is null || string.IsNullOrEmpty(compareApplication))
        {
            return Build(rule, context, FindingStatus.NotApplicable, expected, "No configuration was found to compare.");
        }

        var thisItem = ConfigTreePathResolver.Resolve(context.Tree, rule.TargetPath);
        var otherTree = context.ConfigurationClient.GetConfiguration(new ApplicationConfigurationQuery(
            compareApplication, context.Environment, context.Tenant, context.Namespace, context.SessionYear));
        var otherItem = otherTree is null ? null : ConfigTreePathResolver.Resolve(otherTree, rule.TargetPath);

        if (thisItem is null || otherItem is null)
        {
            return Build(rule, context, FindingStatus.NotApplicable, expected,
                $"{rule.TargetPath} is not configured for one or both applications.");
        }

        var actual = $"{context.Application}='{context.DisplayValue(thisItem)}', {compareApplication}='{context.DisplayValue(otherItem)}'";
        return string.Equals(thisItem.Value, otherItem.Value, StringComparison.Ordinal)
            ? Build(rule, context, FindingStatus.Pass, expected, actual)
            : Build(rule, context, FindingStatus.Warning, expected, actual);
    }
}
