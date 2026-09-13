namespace ConfigLens.Domain.Configuration;

/// <summary>
/// Port representing the "controlled configuration-inspection API" a real
/// Assessor application would expose (CLAUDE.md section 9) - never a raw
/// appsettings.json. Returns null when no configuration is published for the
/// requested combination (a legitimate "missing configuration" scenario, not
/// an error).
/// </summary>
public interface IApplicationConfigurationClient
{
    ConfigTree? GetConfiguration(ApplicationConfigurationQuery query);
}
