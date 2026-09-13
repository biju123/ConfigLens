using System.Reflection;
using System.Text.Json;
using ConfigLens.SampleData.Raw;

namespace ConfigLens.SampleData;

/// <summary>
/// Reads the JSON fixtures under Data/ as embedded resources, so loading
/// them does not depend on the process's working directory (important
/// inside Docker and under test runners alike). Results are cached after
/// first load since the data is static for the lifetime of the process.
/// </summary>
public sealed class SampleDataProvider : ISampleDataProvider
{
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    private readonly Lazy<IReadOnlyList<RawAksResource>> _aksResources =
        new(() => Load<List<RawAksResource>>("aks-inventory.json"));

    private readonly Lazy<IReadOnlyList<RawAppConfigEntry>> _appConfigCatalog =
        new(() => Load<List<RawAppConfigEntry>>("app-config-catalog.json"));

    private readonly Lazy<IReadOnlyList<RawRuleSet>> _ruleSets =
        new(() => [Load<RawRuleSet>("rule-sets/default.json"), Load<RawRuleSet>("rule-sets/strict.json")]);

    private readonly Lazy<IReadOnlyList<RawDependencyScenario>> _dependencyScenarios =
        new(() => Load<List<RawDependencyScenario>>("dependency-scenarios.json"));

    private readonly Lazy<RawReferenceData> _referenceData =
        new(() => Load<RawReferenceData>("reference-data.json"));

    public IReadOnlyList<RawAksResource> GetAksResources() => _aksResources.Value;
    public IReadOnlyList<RawAppConfigEntry> GetAppConfigCatalog() => _appConfigCatalog.Value;
    public IReadOnlyList<RawRuleSet> GetRuleSets() => _ruleSets.Value;
    public IReadOnlyList<RawDependencyScenario> GetDependencyScenarios() => _dependencyScenarios.Value;
    public RawReferenceData GetReferenceData() => _referenceData.Value;

    private static T Load<T>(string relativePath)
    {
        var assembly = typeof(SampleDataProvider).Assembly;

        // MSBuild sanitizes each directory segment of an embedded resource's
        // logical name into a valid identifier (hyphens -> underscores) but
        // leaves the final filename segment untouched - so "rule-sets/x.json"
        // becomes "...Data.rule_sets.x.json", not "...Data.rule-sets.x.json".
        var segments = relativePath.Split('/');
        for (var i = 0; i < segments.Length - 1; i++)
        {
            segments[i] = segments[i].Replace('-', '_');
        }

        var resourceName = "ConfigLens.SampleData.Data." + string.Join('.', segments);
        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException(
                $"Sample data resource '{resourceName}' was not found. Available resources: {string.Join(", ", assembly.GetManifestResourceNames())}");

        return JsonSerializer.Deserialize<T>(stream, JsonOptions)
            ?? throw new InvalidOperationException($"Sample data resource '{resourceName}' deserialized to null.");
    }
}
