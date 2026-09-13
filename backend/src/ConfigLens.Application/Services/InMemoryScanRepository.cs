using System.Collections.Concurrent;
using ConfigLens.Domain.Scan;

namespace ConfigLens.Application.Services;

/// <summary>
/// MVP implementation of IScanRepository. Persistent scan history is not
/// required for the MVP (CLAUDE.md section 12); this keeps scans for the
/// lifetime of the process, which is sufficient to support comparison
/// within a session and can be swapped for a real store later without
/// changing any caller.
/// </summary>
public sealed class InMemoryScanRepository : IScanRepository
{
    private readonly ConcurrentDictionary<ScanId, ScanRecord> _records = new();

    public void Add(ScanRecord record) => _records[record.Metadata.ScanId] = record;

    public ScanRecord? GetById(ScanId scanId) => _records.GetValueOrDefault(scanId);

    public IReadOnlyList<ScanRecord> Query(ScanCategory? category = null) =>
        _records.Values
            .Where(r => category is null || r.Metadata.Category == category)
            .OrderByDescending(r => r.Metadata.StartedAtUtc)
            .ToList();
}
