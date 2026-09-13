namespace ConfigLens.Domain.Scan;

/// <summary>
/// Marker interface implemented by each category's independent result type
/// (AksScanResult, AppConfigScanResult, CharacteristicsScanResult,
/// DependencyScanResult). Deliberately empty: the four categories do not
/// share a generic result shape (see CLAUDE.md section 3).
/// </summary>
public interface IScanResult
{
    ScanCategory Category { get; }
}
