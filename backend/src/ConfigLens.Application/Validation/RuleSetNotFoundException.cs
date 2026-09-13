namespace ConfigLens.Application.Validation;

public sealed class RuleSetNotFoundException(string ruleSetId)
    : InvalidOperationException($"Rule set '{ruleSetId}' was not found.")
{
    public string RuleSetId { get; } = ruleSetId;
}
