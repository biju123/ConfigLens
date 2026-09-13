using ConfigLens.Domain.Comparison;
using ConfigLens.Domain.Configuration;

namespace ConfigLens.Application.Comparison;

/// <summary>
/// Compares two already-masked ConfigTrees by flattening each to
/// dotted-path -> value. Note: because sensitive values are masked to a
/// fixed placeholder before a scan is stored, a changed secret cannot be
/// distinguished from an unchanged one here - that's an accepted trade-off
/// in favor of never persisting or re-exposing real secret values.
/// </summary>
public static class AppConfigComparer
{
    public static AppConfigComparisonResult Compare(ConfigTree current, ConfigTree baseline)
    {
        var currentEntries = Flatten(current);
        var baselineEntries = Flatten(baseline);

        var added = currentEntries.Where(e => !baselineEntries.ContainsKey(e.Key))
            .Select(e => new ConfigEntry(e.Key, e.Value.Value, e.Value.IsSensitive)).ToList();

        var removed = baselineEntries.Where(e => !currentEntries.ContainsKey(e.Key))
            .Select(e => new ConfigEntry(e.Key, e.Value.Value, e.Value.IsSensitive)).ToList();

        var changed = new List<ConfigEntryChange>();
        var unchangedCount = 0;
        foreach (var (path, currentItem) in currentEntries)
        {
            if (!baselineEntries.TryGetValue(path, out var baselineItem))
            {
                continue;
            }

            if (string.Equals(currentItem.Value, baselineItem.Value, StringComparison.Ordinal))
            {
                unchangedCount++;
            }
            else
            {
                changed.Add(new ConfigEntryChange(path, baselineItem.Value, currentItem.Value, currentItem.IsSensitive || baselineItem.IsSensitive));
            }
        }

        return new AppConfigComparisonResult(added, removed, changed, unchangedCount);
    }

    private static Dictionary<string, ConfigItem> Flatten(ConfigTree tree)
    {
        var result = new Dictionary<string, ConfigItem>();
        foreach (var section in tree.Sections)
        {
            FlattenSection(section, section.Name, result);
        }

        return result;
    }

    private static void FlattenSection(ConfigSection section, string path, Dictionary<string, ConfigItem> result)
    {
        foreach (var item in section.Items)
        {
            result[$"{path}.{item.Key}"] = item;
        }

        foreach (var subSection in section.SubSections)
        {
            FlattenSection(subSection, $"{path}.{subSection.Name}", result);
        }
    }
}
