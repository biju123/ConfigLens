using ConfigLens.Domain.Findings;
using ConfigLens.Domain.Scan;
using ConfigLens.Domain.Scan.Characteristics;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Domain.Tests.Scan;

public class CharacteristicsScanResultTests
{
    private static Finding MakeFinding(FindingStatus status) => new()
    {
        FindingId = Guid.NewGuid().ToString(),
        ScanId = ScanId.Create(new DateOnly(2026, 9, 12), 1),
        Category = ScanCategory.ConfigurationCharacteristics,
        Application = "AssessorApi",
        Environment = "Production",
        Resource = "Database.RetryCount",
        RuleId = "r",
        RuleName = "n",
        Severity = FindingSeverity.Medium,
        Status = status,
        Description = "d",
        ExpectedCondition = "e",
        ActualCondition = "a",
        Recommendation = "r"
    };

    [Fact]
    public void Summary_counts_reflect_each_finding_status()
    {
        var result = new CharacteristicsScanResult([
            MakeFinding(FindingStatus.Pass), MakeFinding(FindingStatus.Pass),
            MakeFinding(FindingStatus.Fail), MakeFinding(FindingStatus.Warning),
            MakeFinding(FindingStatus.NotApplicable), MakeFinding(FindingStatus.NotChecked),
            MakeFinding(FindingStatus.Error)
        ]);

        result.PassCount.Should().Be(2);
        result.FailCount.Should().Be(1);
        result.WarningCount.Should().Be(1);
        result.NotApplicableCount.Should().Be(1);
        result.NotCheckedCount.Should().Be(1);
        result.ErrorCount.Should().Be(1);
    }

    [Fact]
    public void Category_is_always_ConfigurationCharacteristics() =>
        new CharacteristicsScanResult([]).Category.Should().Be(ScanCategory.ConfigurationCharacteristics);
}
