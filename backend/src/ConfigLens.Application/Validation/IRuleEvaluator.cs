using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;

namespace ConfigLens.Application.Validation;

public interface IRuleEvaluator
{
    RuleType RuleType { get; }

    Finding Evaluate(ValidationRule rule, RuleEvaluationContext context);
}

/// <summary>Shared Finding-construction helper so individual evaluators only need to decide status/expected/actual.</summary>
public abstract class RuleEvaluatorBase : IRuleEvaluator
{
    public abstract RuleType RuleType { get; }

    public abstract Finding Evaluate(ValidationRule rule, RuleEvaluationContext context);

    protected static Finding Build(
        ValidationRule rule,
        RuleEvaluationContext context,
        FindingStatus status,
        string expectedCondition,
        string actualCondition)
    {
        var resource = rule.TargetPath == ConfigTreePathResolver.RootPath ? "Configuration" : rule.TargetPath;

        return new Finding
        {
            FindingId = $"{context.ScanId}:{rule.RuleId}",
            ScanId = context.ScanId,
            Category = ConfigLens.Domain.Scan.ScanCategory.ConfigurationCharacteristics,
            Application = context.Application,
            Environment = context.Environment,
            Namespace = context.Namespace,
            Resource = resource,
            RuleId = rule.RuleId,
            RuleName = rule.RuleName,
            Severity = rule.Severity,
            Status = status,
            Description = rule.Description,
            ExpectedCondition = expectedCondition,
            ActualCondition = actualCondition,
            Recommendation = status is FindingStatus.Pass or FindingStatus.NotApplicable ? "" : rule.Recommendation
        };
    }
}
