using ConfigLens.Application.Scans.Characteristics;
using ConfigLens.Application.Services;
using ConfigLens.Application.Tests.Fakes;
using ConfigLens.Application.Validation;
using ConfigLens.Application.Validation.Rules;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ConfigLens.Application.Tests.Scans;

public class CharacteristicsScanServiceTests
{
    private static readonly IRuleEvaluator[] Evaluators = [new RequiredExistsRuleEvaluator(), new SessionYearExistsRuleEvaluator()];

    [Fact]
    public void Execute_evaluates_the_requested_rule_set_against_the_applications_configuration()
    {
        var client = new FakeApplicationConfigurationClient().With("AssessorApi", "Production", "ContosoCounty", 2026,
            new ConfigTree("AssessorApi", [new ConfigSection("Database", [new ConfigItem("ConnectionString", "x", true)], [])]));

        var ruleSetProvider = new FakeRuleSetProvider().With(new RuleSet("default", "Default", [
            new ValidationRule { RuleId = "r1", RuleName = "n", RuleType = RuleType.RequiredExists, TargetPath = "Database.RetryCount", Severity = FindingSeverity.High, Description = "d", Recommendation = "r" }
        ]));

        var repository = new InMemoryScanRepository();
        var sut = new CharacteristicsScanService(client, ruleSetProvider, new ValidationEngine(Evaluators), new MaskingService(),
            new ScanIdGenerator(new FakeClock(DateTime.UtcNow)), repository, new FakeClock(DateTime.UtcNow), NullLogger<CharacteristicsScanService>.Instance);

        var response = sut.Execute(new CharacteristicsScanRequest("AssessorApi", "Production", "ContosoCounty", 2026, "default"), "user");

        response.Findings.Should().ContainSingle(f => f.RuleId == "r1" && f.Status == FindingStatus.Fail);
        response.Summary.Fail.Should().Be(1);
    }

    [Fact]
    public void Execute_throws_RuleSetNotFoundException_for_an_unknown_rule_set()
    {
        var sut = new CharacteristicsScanService(
            new FakeApplicationConfigurationClient(), new FakeRuleSetProvider(), new ValidationEngine(Evaluators),
            new MaskingService(), new ScanIdGenerator(new FakeClock(DateTime.UtcNow)), new InMemoryScanRepository(),
            new FakeClock(DateTime.UtcNow), NullLogger<CharacteristicsScanService>.Instance);

        var act = () => sut.Execute(new CharacteristicsScanRequest("AssessorApi", "Production", "ContosoCounty", 2026, "unknown"), "user");

        act.Should().Throw<RuleSetNotFoundException>();
    }
}
