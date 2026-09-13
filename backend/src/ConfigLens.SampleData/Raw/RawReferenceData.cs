namespace ConfigLens.SampleData.Raw;

public sealed class RawRuleSetSummary
{
    public string RuleSetId { get; set; } = "";
    public string Name { get; set; } = "";
}

/// <summary>Backs GET /api/reference-data - populates dropdowns in the scan parameter forms.</summary>
public sealed class RawReferenceData
{
    public List<string> Subscriptions { get; set; } = [];
    public List<string> Clusters { get; set; } = [];
    public List<string> Namespaces { get; set; } = [];
    public List<string> Environments { get; set; } = [];
    public List<string> Tenants { get; set; } = [];
    public List<string> Applications { get; set; } = [];
    public List<int> SessionYears { get; set; } = [];
    public List<RawRuleSetSummary> RuleSets { get; set; } = [];
    public List<string> DependencyTypes { get; set; } = [];
}
