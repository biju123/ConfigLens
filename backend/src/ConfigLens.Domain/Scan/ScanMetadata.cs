namespace ConfigLens.Domain.Scan;

/// <summary>
/// Metadata common to every scan regardless of category. Individual scan
/// results (see IScanResult implementations) carry category-specific data;
/// this type intentionally holds only fields useful for scan history/lookup.
/// </summary>
public sealed record ScanMetadata
{
    public required ScanId ScanId { get; init; }
    public required ScanCategory Category { get; init; }
    public required DateTime StartedAtUtc { get; init; }
    public DateTime? CompletedAtUtc { get; init; }
    public required ScanStatus Status { get; init; }
    public required string InitiatingUser { get; init; }

    public string? Environment { get; init; }
    public string? Tenant { get; init; }
    public string? Application { get; init; }
    public string? Cluster { get; init; }
    public string? Namespace { get; init; }
    public int? SessionYear { get; init; }
}
