using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;

namespace ConfigLens.Application.Validation.Rules;

public sealed class CrossEnvironmentConsistencyRuleEvaluator : RuleEvaluatorBase
{
    public override RuleType RuleType => RuleType.CrossEnvironmentConsistency;

    public override Finding Evaluate(ValidationRule rule, RuleEvaluationContext context)
    {
        var compareEnvironment = rule.Parameters.GetValueOrDefault("compareEnvironment", "");
        var expected = $"{rule.TargetPath} is consistent between {context.Environment} and {compareEnvironment}";

        if (context.Tree is null || string.IsNullOrEmpty(compareEnvironment))
        {
            return Build(rule, context, FindingStatus.NotApplicable, expected, "No configuration was found to compare.");
        }

        var thisItem = ConfigTreePathResolver.Resolve(context.Tree, rule.TargetPath);
        var otherTree = context.ConfigurationClient.GetConfiguration(new ApplicationConfigurationQuery(
            context.Application, compareEnvironment, context.Tenant, context.Namespace, context.SessionYear));
        var otherItem = otherTree is null ? null : ConfigTreePathResolver.Resolve(otherTree, rule.TargetPath);

        if (thisItem is null || otherItem is null)
        {
            return Build(rule, context, FindingStatus.NotApplicable, expected,
                $"{rule.TargetPath} is not configured for one or both environments.");
        }

        var actual = $"{context.Environment}='{context.DisplayValue(thisItem)}', {compareEnvironment}='{context.DisplayValue(otherItem)}'";
        return string.Equals(thisItem.Value, otherItem.Value, StringComparison.Ordinal)
            ? Build(rule, context, FindingStatus.Pass, expected, actual)
            : Build(rule, context, FindingStatus.Warning, expected, actual);
    }
}
