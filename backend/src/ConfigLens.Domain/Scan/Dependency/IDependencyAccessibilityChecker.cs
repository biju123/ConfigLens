namespace ConfigLens.Domain.Scan.Dependency;

/// <summary>
/// Port for testing dependency accessibility. CLAUDE.md section 11 is
/// explicit that ConfigLens reaching a dependency does not prove the
/// application pod can - the MVP implementation reads deterministic sample
/// scenarios; a later implementation would probe from the same
/// network/runtime context as the application.
/// </summary>
public interface IDependencyAccessibilityChecker
{
    IReadOnlyList<DependencyCheckResult> CheckDependencies(DependencyAccessibilityQuery query);
}
