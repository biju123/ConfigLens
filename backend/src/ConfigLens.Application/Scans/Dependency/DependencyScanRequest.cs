using DomainDependencyType = ConfigLens.Domain.Scan.Dependency.DependencyType;

namespace ConfigLens.Application.Scans.Dependency;

public sealed record DependencyScanRequest(
    string Application,
    string Environment,
    string Namespace,
    DomainDependencyType? DependencyType);
