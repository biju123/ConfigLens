namespace ConfigLens.Domain.Reference;

public sealed record RuleSetSummary(string RuleSetId, string Name);

/// <summary>Backs the scan parameter dropdowns in the UI (CLAUDE.md section 7).</summary>
public sealed record ReferenceData(
    IReadOnlyList<string> Subscriptions,
    IReadOnlyList<string> Clusters,
    IReadOnlyList<string> Namespaces,
    IReadOnlyList<string> Environments,
    IReadOnlyList<string> Tenants,
    IReadOnlyList<string> Applications,
    IReadOnlyList<int> SessionYears,
    IReadOnlyList<RuleSetSummary> RuleSets,
    IReadOnlyList<string> DependencyTypes);
