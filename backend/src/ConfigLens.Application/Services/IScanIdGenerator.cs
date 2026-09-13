using ConfigLens.Domain.Scan;

namespace ConfigLens.Application.Services;

public interface IScanIdGenerator
{
    ScanId Next();
}

/// <summary>Per-day sequence counter producing SCAN-yyyyMMdd-NNNNN identifiers (CLAUDE.md section 12).</summary>
public sealed class ScanIdGenerator(IClock clock) : IScanIdGenerator
{
    private readonly System.Collections.Concurrent.ConcurrentDictionary<DateOnly, int> _sequenceByDay = new();

    public ScanId Next()
    {
        var today = DateOnly.FromDateTime(clock.UtcNow);
        var sequence = _sequenceByDay.AddOrUpdate(today, 1, (_, current) => current + 1);
        return ScanId.Create(today, sequence);
    }
}
