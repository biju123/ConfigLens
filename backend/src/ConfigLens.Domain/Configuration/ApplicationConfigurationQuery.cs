namespace ConfigLens.Domain.Configuration;

public sealed record ApplicationConfigurationQuery(
    string Application,
    string Environment,
    string Tenant,
    string? Namespace,
    int SessionYear);
