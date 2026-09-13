using ConfigLens.Domain.Comparison;
using ConfigLens.Domain.Scan.Dependency;

namespace ConfigLens.Application.Comparison;

public static class DependencyComparer
{
    private static readonly DependencyStatus[] FailureStatuses =
    [
        DependencyStatus.ConfigurationMissing, DependencyStatus.ConfigurationInvalid,
        DependencyStatus.Inaccessible, DependencyStatus.Timeout, DependencyStatus.ValidationError
    ];

    public static DependencyComparisonResult Compare(IReadOnlyList<DependencyCheckResult> current, IReadOnlyList<DependencyCheckResult> baseline)
    {
        var currentByKey = current.ToDictionary(Key);
        var baselineByKey = baseline.ToDictionary(Key);
        var allKeys = currentByKey.Keys.Union(baselineByKey.Keys);

        var statusChanges = new List<DependencyStatusChange>();
        var newFailures = new List<DependencyCheckResult>();
        var resolvedFailures = new List<DependencyCheckResult>();

        foreach (var key in allKeys)
        {
            currentByKey.TryGetValue(key, out var currentCheck);
            baselineByKey.TryGetValue(key, out var baselineCheck);

            var currentIsFailure = currentCheck is not null && IsFailure(currentCheck.Status);
            var baselineIsFailure = baselineCheck is not null && IsFailure(baselineCheck.Status);

            if (currentIsFailure && !baselineIsFailure)
            {
                newFailures.Add(currentCheck!);
            }

            if (baselineIsFailure && !currentIsFailure)
            {
                resolvedFailures.Add(baselineCheck!);
            }

            if (currentCheck is not null && baselineCheck is not null && currentCheck.Status != baselineCheck.Status)
            {
                statusChanges.Add(new DependencyStatusChange(key.Application, key.DependencyType, baselineCheck.Status, currentCheck.Status));
            }
        }

        return new DependencyComparisonResult(statusChanges, newFailures, resolvedFailures);
    }

    private static bool IsFailure(DependencyStatus status) => FailureStatuses.Contains(status);

    private static (string Application, DependencyType DependencyType) Key(DependencyCheckResult c) => (c.Application, c.DependencyType);
}
