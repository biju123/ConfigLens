namespace ConfigLens.SampleData.Raw;

public sealed class RawDependencyCheck
{
    public string DependencyType { get; set; } = "";
    public string TargetDescription { get; set; } = "";
    public string Status { get; set; } = "";
    public string? FailureReason { get; set; }
    public int? LatencyMs { get; set; }
}

/// <summary>One entry in dependency-scenarios.json, keyed by Application+Environment+Namespace.</summary>
public sealed class RawDependencyScenario
{
    public string Application { get; set; } = "";
    public string Environment { get; set; } = "";
    public string Namespace { get; set; } = "";
    public List<RawDependencyCheck> Checks { get; set; } = [];
}
