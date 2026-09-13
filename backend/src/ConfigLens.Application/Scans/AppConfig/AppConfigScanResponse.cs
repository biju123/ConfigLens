using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Scan;

namespace ConfigLens.Application.Scans.AppConfig;

public sealed record AppConfigScanResponse(ScanMetadata Metadata, ConfigTree Tree);
