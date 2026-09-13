using ConfigLens.Application.Comparison;
using ConfigLens.Domain.Scan.Dependency;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Application.Tests.Comparison;

public class DependencyComparerTests
{
    private static DependencyCheckResult Check(string app, DependencyType type, DependencyStatus status) => new()
    {
        Application = app,
        DependencyType = type,
        TargetDescription = "target",
        Status = status,
        CheckedAtUtc = DateTime.UtcNow
    };

    [Fact]
    public void Detects_new_failures()
    {
        var current = new[] { Check("AssessorApi", DependencyType.AzureSql, DependencyStatus.Timeout) };
        var baseline = new[] { Check("AssessorApi", DependencyType.AzureSql, DependencyStatus.Accessible) };

        var result = DependencyComparer.Compare(current, baseline);

        result.NewFailures.Should().ContainSingle();
        result.ResolvedFailures.Should().BeEmpty();
    }

    [Fact]
    public void Detects_resolved_failures()
    {
        var current = new[] { Check("AssessorApi", DependencyType.AzureSql, DependencyStatus.Accessible) };
        var baseline = new[] { Check("AssessorApi", DependencyType.AzureSql, DependencyStatus.Timeout) };

        var result = DependencyComparer.Compare(current, baseline);

        result.ResolvedFailures.Should().ContainSingle();
        result.NewFailures.Should().BeEmpty();
    }

    [Fact]
    public void NotApplicable_is_not_treated_as_a_failure()
    {
        var current = new[] { Check("AssessorApi", DependencyType.ExternalApi, DependencyStatus.NotApplicable) };
        var baseline = new[] { Check("AssessorApi", DependencyType.ExternalApi, DependencyStatus.Accessible) };

        var result = DependencyComparer.Compare(current, baseline);

        result.NewFailures.Should().BeEmpty();
        result.StatusChanges.Should().ContainSingle();
    }
}
