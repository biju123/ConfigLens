namespace ConfigLens.Domain.Scan.Dependency;

public sealed record DependencyCheckResult
{
    public required string Application { get; init; }
    public required DependencyType DependencyType { get; init; }

    /// <summary>Non-sensitive description of the target (e.g. host/account name only - never a full connection string).</summary>
    public required string TargetDescription { get; init; }

    public required DependencyStatus Status { get; init; }
    public string? FailureReason { get; init; }
    public required DateTime CheckedAtUtc { get; init; }
    public int? LatencyMs { get; init; }
}
