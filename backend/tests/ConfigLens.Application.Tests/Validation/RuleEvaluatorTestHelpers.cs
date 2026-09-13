using ConfigLens.Application.Services;
using ConfigLens.Application.Tests.Fakes;
using ConfigLens.Application.Validation;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Scan;

namespace ConfigLens.Application.Tests.Validation;

internal static class RuleEvaluatorTestHelpers
{
    public static readonly ScanId TestScanId = ScanId.Create(new DateOnly(2026, 9, 12), 1);

    public static ValidationRule Rule(RuleType type, string targetPath, Dictionary<string, string>? parameters = null) => new()
    {
        RuleId = "test-rule",
        RuleName = "Test Rule",
        RuleType = type,
        TargetPath = targetPath,
        Severity = ConfigLens.Domain.Findings.FindingSeverity.Medium,
        Description = "description",
        Recommendation = "recommendation",
        Parameters = parameters ?? new Dictionary<string, string>()
    };

    public static RuleEvaluationContext Context(
        ConfigTree? tree,
        string application = "AssessorApi",
        string environment = "Production",
        string tenant = "ContosoCounty",
        int sessionYear = 2026,
        FakeApplicationConfigurationClient? client = null) =>
        new(TestScanId, application, environment, tenant, null, sessionYear, tree, client ?? new FakeApplicationConfigurationClient(), new MaskingService());

    public static ConfigTree TreeWith(string sectionName, params ConfigItem[] items) =>
        new("AssessorApi", [new ConfigSection(sectionName, items, [])]);
}
