using ConfigLens.Domain.Scan;

namespace ConfigLens.Domain.Findings;

/// <summary>
/// The outcome of evaluating one ValidationRule against collected
/// configuration. Field list matches CLAUDE.md section 4 exactly.
/// </summary>
public sealed record Finding
{
    public required string FindingId { get; init; }
    public required ScanId ScanId { get; init; }
    public required ScanCategory Category { get; init; }
    public required string Application { get; init; }
    public required string Environment { get; init; }
    public string? Namespace { get; init; }
    public required string Resource { get; init; }
    public required string RuleId { get; init; }
    public required string RuleName { get; init; }
    public required FindingSeverity Severity { get; init; }
    public required FindingStatus Status { get; init; }
    public required string Description { get; init; }
    public required string ExpectedCondition { get; init; }
    public required string ActualCondition { get; init; }
    public required string Recommendation { get; init; }
}
