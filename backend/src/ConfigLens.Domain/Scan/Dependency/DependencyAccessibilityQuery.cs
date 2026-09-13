namespace ConfigLens.Domain.Scan.Dependency;

public sealed record DependencyAccessibilityQuery(
    string Application,
    string Environment,
    string Namespace,
    DependencyType? DependencyType);
