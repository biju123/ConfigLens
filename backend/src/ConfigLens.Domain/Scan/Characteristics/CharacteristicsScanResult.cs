using ConfigLens.Domain.Findings;

namespace ConfigLens.Domain.Scan.Characteristics;

public sealed record CharacteristicsScanResult(IReadOnlyList<Finding> Findings) : IScanResult
{
    public ScanCategory Category => ScanCategory.ConfigurationCharacteristics;

    public int PassCount => Count(FindingStatus.Pass);
    public int WarningCount => Count(FindingStatus.Warning);
    public int FailCount => Count(FindingStatus.Fail);
    public int NotCheckedCount => Count(FindingStatus.NotChecked);
    public int NotApplicableCount => Count(FindingStatus.NotApplicable);
    public int ErrorCount => Count(FindingStatus.Error);

    private int Count(FindingStatus status) => Findings.Count(f => f.Status == status);
}
