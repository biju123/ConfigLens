using ConfigLens.Domain.Findings;

namespace ConfigLens.Domain.Comparison;

public sealed record FindingStatusChange(string RuleId, string Resource, FindingStatus OldStatus, FindingStatus NewStatus);

public enum RuleChangeType
{
    Added,
    Removed,
    Modified
}

public sealed record RuleChange(string RuleId, string RuleName, RuleChangeType ChangeType);

public sealed record CharacteristicsComparisonResult(
    IReadOnlyList<Finding> NewFailures,
    IReadOnlyList<Finding> ResolvedFailures,
    IReadOnlyList<Finding> NewWarnings,
    IReadOnlyList<Finding> ResolvedWarnings,
    IReadOnlyList<FindingStatusChange> StatusChanges,
    IReadOnlyList<RuleChange> RuleChanges);
