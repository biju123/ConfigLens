using ConfigLens.Domain.Configuration;

namespace ConfigLens.Application.Validation;

/// <summary>Resolves a dotted rule TargetPath (e.g. "Database.ConnectionString") against a ConfigTree.</summary>
internal static class ConfigTreePathResolver
{
    public const string RootPath = "$root";

    public static ConfigItem? Resolve(ConfigTree tree, string targetPath)
    {
        var segments = targetPath.Split('.', StringSplitOptions.RemoveEmptyEntries);
        if (segments.Length < 2)
        {
            return null;
        }

        IReadOnlyList<ConfigSection> sections = tree.Sections;
        ConfigSection? section = null;
        for (var i = 0; i < segments.Length - 1; i++)
        {
            section = sections.FirstOrDefault(s => string.Equals(s.Name, segments[i], StringComparison.OrdinalIgnoreCase));
            if (section is null)
            {
                return null;
            }

            sections = section.SubSections;
        }

        var itemKey = segments[^1];
        return section?.Items.FirstOrDefault(i => string.Equals(i.Key, itemKey, StringComparison.OrdinalIgnoreCase));
    }
}
