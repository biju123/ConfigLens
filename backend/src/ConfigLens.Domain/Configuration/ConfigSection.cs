namespace ConfigLens.Domain.Configuration;

/// <summary>
/// A node in the hierarchical configuration tree (e.g. "Database", holding
/// ConnectionString/CommandTimeout/RetryCount items and no sub-sections).
/// </summary>
public sealed record ConfigSection(
    string Name,
    IReadOnlyList<ConfigItem> Items,
    IReadOnlyList<ConfigSection> SubSections);
