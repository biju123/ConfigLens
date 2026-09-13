using ConfigLens.Domain.Scan;

namespace ConfigLens.Application.Comparison;

/// <summary>
/// Transport envelope only - Result holds one of the four independent
/// *ComparisonResult types and is serialized as-is, so the JSON shape stays
/// category-specific even though one endpoint/service handles dispatch.
/// </summary>
public sealed record ComparisonResponse(ScanId CurrentScanId, ScanId BaselineScanId, ScanCategory Category, object Result);
