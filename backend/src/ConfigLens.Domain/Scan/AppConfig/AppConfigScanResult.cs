using ConfigLens.Domain.Configuration;

namespace ConfigLens.Domain.Scan.AppConfig;

public sealed record AppConfigScanResult(ConfigTree Tree) : IScanResult
{
    public ScanCategory Category => ScanCategory.ApplicationConfiguration;
}
