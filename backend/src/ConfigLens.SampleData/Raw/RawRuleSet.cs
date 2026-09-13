namespace ConfigLens.SampleData.Raw;

public sealed class RawValidationRule
{
    public string RuleId { get; set; } = "";
    public string RuleName { get; set; } = "";
    public string RuleType { get; set; } = "";
    public string TargetPath { get; set; } = "";
    public string Severity { get; set; } = "";
    public string Description { get; set; } = "";
    public string Recommendation { get; set; } = "";
    public Dictionary<string, string> Parameters { get; set; } = [];
}

public sealed class RawRuleSet
{
    public string RuleSetId { get; set; } = "";
    public string Name { get; set; } = "";
    public List<RawValidationRule> Rules { get; set; } = [];
}
