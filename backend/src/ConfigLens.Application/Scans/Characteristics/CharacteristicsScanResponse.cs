using ConfigLens.Domain.Findings;
using ConfigLens.Domain.Scan;

namespace ConfigLens.Application.Scans.Characteristics;

public sealed record CharacteristicsSummaryCounts(int Pass, int Warning, int Fail, int NotChecked, int NotApplicable, int Error);

public sealed record CharacteristicsScanResponse(ScanMetadata Metadata, IReadOnlyList<Finding> Findings, CharacteristicsSummaryCounts Summary);
