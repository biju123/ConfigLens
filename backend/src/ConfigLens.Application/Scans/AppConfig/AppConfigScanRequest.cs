namespace ConfigLens.Application.Scans.AppConfig;

public sealed record AppConfigScanRequest(
    string Application,
    string Environment,
    string Tenant,
    string? Namespace,
    int SessionYear);
