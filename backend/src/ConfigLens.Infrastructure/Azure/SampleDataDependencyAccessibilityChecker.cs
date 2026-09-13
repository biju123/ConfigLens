using ConfigLens.Domain.Scan.Dependency;
using ConfigLens.SampleData;
using ConfigLens.SampleData.Raw;

namespace ConfigLens.Infrastructure.Azure;

public sealed class SampleDataDependencyAccessibilityChecker(ISampleDataProvider sampleData) : IDependencyAccessibilityChecker
{
    public IReadOnlyList<DependencyCheckResult> CheckDependencies(DependencyAccessibilityQuery query)
    {
        var scenario = sampleData.GetDependencyScenarios().FirstOrDefault(s =>
            string.Equals(s.Application, query.Application, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(s.Environment, query.Environment, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(s.Namespace, query.Namespace, StringComparison.OrdinalIgnoreCase));

        if (scenario is null)
        {
            return [];
        }

        var checkedAtUtc = DateTime.UtcNow;

        return scenario.Checks
            .Select(c => Map(query.Application, c, checkedAtUtc))
            .Where(c => query.DependencyType is null || c.DependencyType == query.DependencyType)
            .ToList();
    }

    private static DependencyCheckResult Map(string application, RawDependencyCheck raw, DateTime checkedAtUtc) => new()
    {
        Application = application,
        DependencyType = Enum.Parse<DependencyType>(raw.DependencyType, ignoreCase: true),
        TargetDescription = raw.TargetDescription,
        Status = Enum.Parse<DependencyStatus>(raw.Status, ignoreCase: true),
        FailureReason = raw.FailureReason,
        CheckedAtUtc = checkedAtUtc,
        LatencyMs = raw.LatencyMs
    };
}
