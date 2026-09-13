namespace ConfigLens.Domain.Comparison;

public sealed record ConfigEntry(string Path, string? Value, bool IsSensitive);

public sealed record ConfigEntryChange(string Path, string? OldValue, string? NewValue, bool IsSensitive);

public sealed record AppConfigComparisonResult(
    IReadOnlyList<ConfigEntry> Added,
    IReadOnlyList<ConfigEntry> Removed,
    IReadOnlyList<ConfigEntryChange> Changed,
    int UnchangedCount);
