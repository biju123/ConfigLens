using ConfigLens.Domain.Scan;
using ConfigLens.Domain.Scan.Dependency;

namespace ConfigLens.Application.Scans.Dependency;

public sealed record DependencyScanResponse(ScanMetadata Metadata, IReadOnlyList<DependencyCheckResult> Checks);
