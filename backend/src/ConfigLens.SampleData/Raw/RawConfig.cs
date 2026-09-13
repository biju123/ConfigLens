namespace ConfigLens.SampleData.Raw;

public sealed class RawConfigItem
{
    public string Key { get; set; } = "";
    public string? Value { get; set; }
    public bool Sensitive { get; set; }
}

public sealed class RawConfigSection
{
    public string Name { get; set; } = "";
    public List<RawConfigItem> Items { get; set; } = [];
    public List<RawConfigSection> SubSections { get; set; } = [];
}

/// <summary>One entry in app-config-catalog.json, keyed by Application+Environment+Tenant+SessionYear.</summary>
public sealed class RawAppConfigEntry
{
    public string Application { get; set; } = "";
    public string Environment { get; set; } = "";
    public string Tenant { get; set; } = "";
    public int SessionYear { get; set; }
    public List<RawConfigSection> Sections { get; set; } = [];
}
