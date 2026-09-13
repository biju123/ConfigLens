using ConfigLens.Application.Comparison;
using ConfigLens.Domain.Comparison;
using ConfigLens.Domain.Findings;
using ConfigLens.Domain.Scan;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Application.Tests.Comparison;

public class CharacteristicsComparerTests
{
    private static Finding MakeFinding(string ruleId, string resource, FindingStatus status, string ruleName = "Rule", FindingSeverity severity = FindingSeverity.Medium) => new()
    {
        FindingId = $"scan:{ruleId}",
        ScanId = ScanId.Create(new DateOnly(2026, 9, 12), 1),
        Category = ScanCategory.ConfigurationCharacteristics,
        Application = "AssessorApi",
        Environment = "Production",
        Resource = resource,
        RuleId = ruleId,
        RuleName = ruleName,
        Severity = severity,
        Status = status,
        Description = "d",
        ExpectedCondition = "e",
        ActualCondition = "a",
        Recommendation = "r"
    };

    [Fact]
    public void Detects_new_and_resolved_failures()
    {
        var current = new[] { MakeFinding("r1", "Database.RetryCount", FindingStatus.Fail) };
        var baseline = new[] { MakeFinding("r2", "ServiceBus.Queue", FindingStatus.Fail) };

        var result = CharacteristicsComparer.Compare(current, baseline);

        result.NewFailures.Should().ContainSingle(f => f.RuleId == "r1");
        result.ResolvedFailures.Should().ContainSingle(f => f.RuleId == "r2");
    }

    [Fact]
    public void Detects_status_changes_for_matching_findings()
    {
        var current = new[] { MakeFinding("r1", "Database.RetryCount", FindingStatus.Pass) };
        var baseline = new[] { MakeFinding("r1", "Database.RetryCount", FindingStatus.Fail) };

        var result = CharacteristicsComparer.Compare(current, baseline);

        result.StatusChanges.Should().ContainSingle(c => c.OldStatus == FindingStatus.Fail && c.NewStatus == FindingStatus.Pass);
        result.ResolvedFailures.Should().ContainSingle(f => f.RuleId == "r1");
    }

    [Fact]
    public void Detects_added_removed_and_modified_rules()
    {
        var current = new[]
        {
            MakeFinding("r1", "Database.RetryCount", FindingStatus.Pass, severity: FindingSeverity.High),
            MakeFinding("r3", "Storage.Account", FindingStatus.Pass)
        };
        var baseline = new[]
        {
            MakeFinding("r1", "Database.RetryCount", FindingStatus.Pass, severity: FindingSeverity.Medium),
            MakeFinding("r2", "ServiceBus.Queue", FindingStatus.Pass)
        };

        var result = CharacteristicsComparer.Compare(current, baseline);

        result.RuleChanges.Should().Contain(c => c.RuleId == "r1" && c.ChangeType == RuleChangeType.Modified);
        result.RuleChanges.Should().Contain(c => c.RuleId == "r2" && c.ChangeType == RuleChangeType.Removed);
        result.RuleChanges.Should().Contain(c => c.RuleId == "r3" && c.ChangeType == RuleChangeType.Added);
    }
}
