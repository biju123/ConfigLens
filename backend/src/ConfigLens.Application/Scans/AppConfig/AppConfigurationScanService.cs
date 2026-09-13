using System.Diagnostics;
using ConfigLens.Application.Services;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Scan;
using ConfigLens.Domain.Scan.AppConfig;
using Microsoft.Extensions.Logging;

namespace ConfigLens.Application.Scans.AppConfig;

/// <summary>
/// Masks the configuration tree before it is stored or returned - the
/// scan result held in IScanRepository (and therefore any later comparison)
/// only ever contains masked values, matching CLAUDE.md section 9's
/// requirement that sensitive values are never returned.
/// </summary>
public sealed class AppConfigurationScanService(
    IApplicationConfigurationClient configurationClient,
    IMaskingService maskingService,
    IScanIdGenerator scanIdGenerator,
    IScanRepository scanRepository,
    IClock clock,
    ILogger<AppConfigurationScanService> logger)
{
    public AppConfigScanResponse Execute(AppConfigScanRequest request, string initiatingUser)
    {
        var stopwatch = Stopwatch.StartNew();
        var startedAtUtc = clock.UtcNow;
        var scanId = scanIdGenerator.Next();

        var rawTree = configurationClient.GetConfiguration(new ApplicationConfigurationQuery(
            request.Application, request.Environment, request.Tenant, request.Namespace, request.SessionYear));

        var maskedTree = rawTree is null
            ? new ConfigTree(request.Application, [])
            : maskingService.MaskTree(rawTree);

        var metadata = new ScanMetadata
        {
            ScanId = scanId,
            Category = ScanCategory.ApplicationConfiguration,
            StartedAtUtc = startedAtUtc,
            CompletedAtUtc = clock.UtcNow,
            Status = ScanStatus.Completed,
            InitiatingUser = initiatingUser,
            Application = request.Application,
            Environment = request.Environment,
            Tenant = request.Tenant,
            Namespace = request.Namespace,
            SessionYear = request.SessionYear
        };

        scanRepository.Add(new ScanRecord(metadata, new AppConfigScanResult(maskedTree)));
        stopwatch.Stop();

        logger.LogInformation(
            "Scan {ScanId} category {Category} application {Application} environment {Environment} operation {Operation} completed in {DurationMs}ms result {Result}",
            scanId, ScanCategory.ApplicationConfiguration, request.Application, request.Environment,
            "ApplicationConfigurationScan", stopwatch.ElapsedMilliseconds, rawTree is null ? "not found" : "found");

        return new AppConfigScanResponse(metadata, maskedTree);
    }
}
