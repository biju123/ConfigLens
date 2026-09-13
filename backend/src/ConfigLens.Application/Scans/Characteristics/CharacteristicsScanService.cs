using System.Diagnostics;
using ConfigLens.Application.Services;
using ConfigLens.Application.Validation;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Scan;
using ConfigLens.Domain.Scan.Characteristics;
using Microsoft.Extensions.Logging;

namespace ConfigLens.Application.Scans.Characteristics;

public sealed class CharacteristicsScanService(
    IApplicationConfigurationClient configurationClient,
    IRuleSetProvider ruleSetProvider,
    IValidationEngine validationEngine,
    IMaskingService maskingService,
    IScanIdGenerator scanIdGenerator,
    IScanRepository scanRepository,
    IClock clock,
    ILogger<CharacteristicsScanService> logger)
{
    public CharacteristicsScanResponse Execute(CharacteristicsScanRequest request, string initiatingUser)
    {
        var stopwatch = Stopwatch.StartNew();
        var startedAtUtc = clock.UtcNow;
        var scanId = scanIdGenerator.Next();

        var ruleSet = ruleSetProvider.GetRuleSet(request.RuleSetId) ?? throw new RuleSetNotFoundException(request.RuleSetId);

        var tree = configurationClient.GetConfiguration(new ApplicationConfigurationQuery(
            request.Application, request.Environment, request.Tenant, null, request.SessionYear));

        var context = new RuleEvaluationContext(
            scanId, request.Application, request.Environment, request.Tenant, null, request.SessionYear,
            tree, configurationClient, maskingService);

        var findings = validationEngine.Evaluate(ruleSet, context);
        var result = new CharacteristicsScanResult(findings);

        var metadata = new ScanMetadata
        {
            ScanId = scanId,
            Category = ScanCategory.ConfigurationCharacteristics,
            StartedAtUtc = startedAtUtc,
            CompletedAtUtc = clock.UtcNow,
            Status = ScanStatus.Completed,
            InitiatingUser = initiatingUser,
            Application = request.Application,
            Environment = request.Environment,
            Tenant = request.Tenant,
            SessionYear = request.SessionYear
        };

        scanRepository.Add(new ScanRecord(metadata, result));
        stopwatch.Stop();

        logger.LogInformation(
            "Scan {ScanId} category {Category} application {Application} environment {Environment} operation {Operation} completed in {DurationMs}ms result {Result}",
            scanId, ScanCategory.ConfigurationCharacteristics, request.Application, request.Environment,
            "ConfigurationCharacteristicsScan", stopwatch.ElapsedMilliseconds,
            $"{result.PassCount} pass / {result.WarningCount} warn / {result.FailCount} fail");

        var summary = new CharacteristicsSummaryCounts(
            result.PassCount, result.WarningCount, result.FailCount,
            result.NotCheckedCount, result.NotApplicableCount, result.ErrorCount);

        return new CharacteristicsScanResponse(metadata, findings, summary);
    }
}
