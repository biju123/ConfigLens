namespace ConfigLens.Domain.Scan;

/// <summary>
/// Retains scan metadata/results for later lookup and comparison. The MVP
/// backs this with an in-memory implementation; the interface is shaped so a
/// persistent store can replace it later without changing callers.
/// </summary>
public interface IScanRepository
{
    void Add(ScanRecord record);

    ScanRecord? GetById(ScanId scanId);

    IReadOnlyList<ScanRecord> Query(ScanCategory? category = null);
}
