using ConfigLens.Domain.Configuration;

namespace ConfigLens.Application.Services;

public interface IMaskingService
{
    bool IsSensitiveKey(string key);

    ConfigItem MaskIfSensitive(ConfigItem item);

    ConfigTree MaskTree(ConfigTree tree);
}

/// <summary>
/// Full-masks any configuration value whose key looks sensitive. A simple
/// case-insensitive substring match against a fixed keyword list is
/// deliberately used instead of partial reveal or format-aware masking -
/// CLAUDE.md section 19 requires values are never exposed, and the simplest
/// rule that can't leak a fragment of a secret is the safest one.
/// </summary>
public sealed class MaskingService : IMaskingService
{
    private const string MaskedValue = "********";

    private static readonly string[] SensitiveKeywords =
        ["password", "secret", "apikey", "api-key", "token", "connectionstring", "credential"];

    public bool IsSensitiveKey(string key) =>
        SensitiveKeywords.Any(keyword => key.Contains(keyword, StringComparison.OrdinalIgnoreCase));

    public ConfigItem MaskIfSensitive(ConfigItem item) =>
        IsSensitiveKey(item.Key)
            ? item with { Value = item.Value is null ? null : MaskedValue, IsSensitive = true }
            : item;

    public ConfigTree MaskTree(ConfigTree tree) => tree with { Sections = tree.Sections.Select(MaskSection).ToList() };

    private ConfigSection MaskSection(ConfigSection section) => section with
    {
        Items = section.Items.Select(MaskIfSensitive).ToList(),
        SubSections = section.SubSections.Select(MaskSection).ToList()
    };
}
