using System.Diagnostics;
using ConfigLens.Application.Services;
using ConfigLens.Domain.Scan;
using ConfigLens.Domain.Scan.Aks;
using Microsoft.Extensions.Logging;

namespace ConfigLens.Application.Scans.Aks;

public sealed class AksDeploymentScanService(
    IKubernetesInventoryReader inventoryReader,
    IScanIdGenerator scanIdGenerator,
    IScanRepository scanRepository,
    IClock clock,
    ILogger<AksDeploymentScanService> logger)
{
    public AksScanResponse Execute(AksScanRequest request, string initiatingUser)
    {
        var stopwatch = Stopwatch.StartNew();
        var startedAtUtc = clock.UtcNow;
        var scanId = scanIdGenerator.Next();

        var resources = inventoryReader.GetResources(new AksInventoryQuery(
            request.SubscriptionId, request.ClusterName, request.Environment, request.Tenant, request.Namespace));

        var metadata = new ScanMetadata
        {
            ScanId = scanId,
            Category = ScanCategory.AksDeployment,
            StartedAtUtc = startedAtUtc,
            CompletedAtUtc = clock.UtcNow,
            Status = ScanStatus.Completed,
            InitiatingUser = initiatingUser,
            Environment = request.Environment,
            Tenant = request.Tenant,
            Cluster = request.ClusterName,
            Namespace = request.Namespace
        };

        scanRepository.Add(new ScanRecord(metadata, new AksScanResult(resources)));
        stopwatch.Stop();

        logger.LogInformation(
            "Scan {ScanId} category {Category} environment {Environment} namespace {Namespace} operation {Operation} completed in {DurationMs}ms result {Result}",
            scanId, ScanCategory.AksDeployment, request.Environment, request.Namespace, "AksDeploymentScan",
            stopwatch.ElapsedMilliseconds, $"{resources.Count} resources");

        return new AksScanResponse(metadata, resources);
    }
}
