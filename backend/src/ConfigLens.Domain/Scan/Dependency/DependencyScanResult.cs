namespace ConfigLens.Domain.Scan.Dependency;

public sealed record DependencyScanResult(IReadOnlyList<DependencyCheckResult> Checks) : IScanResult
{
    public ScanCategory Category => ScanCategory.DependencyAccessibility;
}
