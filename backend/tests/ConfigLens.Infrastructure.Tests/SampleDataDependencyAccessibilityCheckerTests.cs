using ConfigLens.Domain.Scan.Dependency;
using ConfigLens.Infrastructure.Azure;
using ConfigLens.SampleData;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Infrastructure.Tests;

public class SampleDataDependencyAccessibilityCheckerTests
{
    private readonly SampleDataDependencyAccessibilityChecker _sut = new(new SampleDataProvider());

    [Fact]
    public void CheckDependencies_returns_every_dependency_type_when_no_filter_given()
    {
        var results = _sut.CheckDependencies(new DependencyAccessibilityQuery("AssessorApi", "Production", "assessor-prod", null));

        results.Should().HaveCount(6);
    }

    [Fact]
    public void CheckDependencies_filters_by_dependency_type()
    {
        var results = _sut.CheckDependencies(new DependencyAccessibilityQuery("AssessorApi", "Production", "assessor-prod", DependencyType.AzureKeyVault));

        results.Should().ContainSingle().Which.DependencyType.Should().Be(DependencyType.AzureKeyVault);
    }

    [Fact]
    public void Sample_scenarios_cover_every_dependency_status_at_least_once()
    {
        var allStatuses = new[]
        {
            _sut.CheckDependencies(new DependencyAccessibilityQuery("AssessorApi", "Production", "assessor-prod", null)),
            _sut.CheckDependencies(new DependencyAccessibilityQuery("AssessorApi", "Staging", "assessor-staging", null)),
            _sut.CheckDependencies(new DependencyAccessibilityQuery("AssessorApi", "Production", "assessor-prod-riverbend", null))
        }.SelectMany(r => r).Select(r => r.Status).ToHashSet();

        allStatuses.Should().Contain(Enum.GetValues<DependencyStatus>());
    }

    [Fact]
    public void CheckDependencies_returns_empty_for_unknown_scenario()
    {
        _sut.CheckDependencies(new DependencyAccessibilityQuery("Unknown", "Production", "nowhere", null)).Should().BeEmpty();
    }
}
