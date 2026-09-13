using ConfigLens.Domain.Findings;

namespace ConfigLens.Domain.Configuration;

/// <summary>
/// A single validation rule, expressed as pure data so rule sets can be
/// authored/extended without changing code (CLAUDE.md section 10: "rules
/// must not be implemented directly inside React components" - they live
/// here, in the backend domain, as data evaluated by the validation engine).
/// </summary>
public sealed record ValidationRule
{
    public required string RuleId { get; init; }
    public required string RuleName { get; init; }
    public required RuleType RuleType { get; init; }

    /// <summary>Dotted path into the ConfigTree this rule targets, e.g. "Database.ConnectionString".</summary>
    public required string TargetPath { get; init; }

    public required FindingSeverity Severity { get; init; }
    public required string Description { get; init; }
    public required string Recommendation { get; init; }

    /// <summary>Rule-type-specific parameters (e.g. "pattern" for FormatMatch, "min"/"max" for NumericRange, "allowedValues" for AllowedValues).</summary>
    public IReadOnlyDictionary<string, string> Parameters { get; init; } = new Dictionary<string, string>();
}
