using System.Diagnostics;
using ConfigLens.Application.Services;
using ConfigLens.Domain.Scan;
using ConfigLens.Domain.Scan.Dependency;
using Microsoft.Extensions.Logging;

namespace ConfigLens.Application.Scans.Dependency;

public sealed class DependencyAccessibilityScanService(
    IDependencyAccessibilityChecker checker,
    IScanIdGenerator scanIdGenerator,
    IScanRepository scanRepository,
    IClock clock,
    ILogger<DependencyAccessibilityScanService> logger)
{
    public DependencyScanResponse Execute(DependencyScanRequest request, string initiatingUser)
    {
        var stopwatch = Stopwatch.StartNew();
        var startedAtUtc = clock.UtcNow;
        var scanId = scanIdGenerator.Next();

        var checks = checker.CheckDependencies(new DependencyAccessibilityQuery(
            request.Application, request.Environment, request.Namespace, request.DependencyType));

        var metadata = new ScanMetadata
        {
            ScanId = scanId,
            Category = ScanCategory.DependencyAccessibility,
            StartedAtUtc = startedAtUtc,
            CompletedAtUtc = clock.UtcNow,
            Status = ScanStatus.Completed,
            InitiatingUser = initiatingUser,
            Application = request.Application,
            Environment = request.Environment,
            Namespace = request.Namespace
        };

        scanRepository.Add(new ScanRecord(metadata, new DependencyScanResult(checks)));
        stopwatch.Stop();

        logger.LogInformation(
            "Scan {ScanId} category {Category} application {Application} environment {Environment} namespace {Namespace} operation {Operation} completed in {DurationMs}ms result {Result}",
            scanId, ScanCategory.DependencyAccessibility, request.Application, request.Environment, request.Namespace,
            "DependencyAccessibilityScan", stopwatch.ElapsedMilliseconds, $"{checks.Count} checks");

        return new DependencyScanResponse(metadata, checks);
    }
}
