using ConfigLens.Domain.Scan.Dependency;

namespace ConfigLens.Domain.Comparison;

public sealed record DependencyStatusChange(
    string Application,
    DependencyType DependencyType,
    DependencyStatus OldStatus,
    DependencyStatus NewStatus);

public sealed record DependencyComparisonResult(
    IReadOnlyList<DependencyStatusChange> StatusChanges,
    IReadOnlyList<DependencyCheckResult> NewFailures,
    IReadOnlyList<DependencyCheckResult> ResolvedFailures);
