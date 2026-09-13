using ConfigLens.Domain.Configuration;
using ConfigLens.SampleData;
using ConfigLens.SampleData.Raw;

namespace ConfigLens.Infrastructure.ApplicationApis;

/// <summary>
/// Stands in for the "controlled configuration-inspection API" a real
/// Assessor application would expose. Values here are never masked - the
/// caller (Application.Services.IMaskingService, used from the scan
/// services) is responsible for masking before data leaves the Application
/// layer, since Characteristics validation needs the real values while
/// Application Configuration scan results must not.
/// </summary>
public sealed class SampleDataApplicationConfigurationClient(ISampleDataProvider sampleData) : IApplicationConfigurationClient
{
    public ConfigTree? GetConfiguration(ApplicationConfigurationQuery query)
    {
        var entry = sampleData.GetAppConfigCatalog().FirstOrDefault(e =>
            string.Equals(e.Application, query.Application, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(e.Environment, query.Environment, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(e.Tenant, query.Tenant, StringComparison.OrdinalIgnoreCase) &&
            e.SessionYear == query.SessionYear);

        return entry is null ? null : new ConfigTree(entry.Application, entry.Sections.Select(MapSection).ToList());
    }

    private static ConfigSection MapSection(RawConfigSection raw) => new(
        raw.Name,
        raw.Items.Select(i => new ConfigItem(i.Key, i.Value, i.Sensitive)).ToList(),
        raw.SubSections.Select(MapSection).ToList());
}
