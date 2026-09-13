using ConfigLens.Domain.Scan.Dependency;

namespace ConfigLens.Application.Tests.Fakes;

public sealed class FakeDependencyAccessibilityChecker(IReadOnlyList<DependencyCheckResult> results) : IDependencyAccessibilityChecker
{
    public IReadOnlyList<DependencyCheckResult> CheckDependencies(DependencyAccessibilityQuery query) => results;
}
