using ConfigLens.Domain.Scan;
using ConfigLens.Domain.Scan.Aks;

namespace ConfigLens.Application.Scans.Aks;

public sealed record AksScanResponse(ScanMetadata Metadata, IReadOnlyList<AksResource> Resources);
