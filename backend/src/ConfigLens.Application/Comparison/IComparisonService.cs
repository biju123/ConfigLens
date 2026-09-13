using ConfigLens.Domain.Comparison;
using ConfigLens.Domain.Scan;
using ConfigLens.Domain.Scan.AppConfig;
using ConfigLens.Domain.Scan.Aks;
using ConfigLens.Domain.Scan.Characteristics;
using ConfigLens.Domain.Scan.Dependency;

namespace ConfigLens.Application.Comparison;

public interface IComparisonService
{
    ComparisonResponse Compare(CompareScansRequest request);
}

/// <summary>
/// Looks up both scans, enforces same-category comparison (CLAUDE.md
/// section 13), then dispatches to the category-specific comparer. Adding a
/// fifth category means adding one switch arm here, not touching the
/// existing four comparers.
/// </summary>
public sealed class ComparisonService(IScanRepository scanRepository) : IComparisonService
{
    public ComparisonResponse Compare(CompareScansRequest request)
    {
        var currentId = ScanId.Parse(request.CurrentScanId);
        var baselineId = ScanId.Parse(request.BaselineScanId);

        var currentRecord = scanRepository.GetById(currentId) ?? throw new ScanNotFoundException(currentId);
        var baselineRecord = scanRepository.GetById(baselineId) ?? throw new ScanNotFoundException(baselineId);

        if (currentRecord.Metadata.Category != baselineRecord.Metadata.Category)
        {
            throw new CategoryMismatchException(currentRecord.Metadata.Category, baselineRecord.Metadata.Category);
        }

        var category = currentRecord.Metadata.Category;
        object result = category switch
        {
            ScanCategory.AksDeployment => AksComparer.Compare(
                ((AksScanResult)currentRecord.Result).Resources, ((AksScanResult)baselineRecord.Result).Resources),

            ScanCategory.ApplicationConfiguration => AppConfigComparer.Compare(
                ((AppConfigScanResult)currentRecord.Result).Tree, ((AppConfigScanResult)baselineRecord.Result).Tree),

            ScanCategory.ConfigurationCharacteristics => CharacteristicsComparer.Compare(
                ((CharacteristicsScanResult)currentRecord.Result).Findings, ((CharacteristicsScanResult)baselineRecord.Result).Findings),

            ScanCategory.DependencyAccessibility => DependencyComparer.Compare(
                ((DependencyScanResult)currentRecord.Result).Checks, ((DependencyScanResult)baselineRecord.Result).Checks),

            _ => throw new InvalidOperationException($"Unsupported scan category '{category}'.")
        };

        return new ComparisonResponse(currentId, baselineId, category, result);
    }
}
