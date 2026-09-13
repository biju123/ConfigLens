using ConfigLens.Infrastructure.ApplicationApis;
using ConfigLens.SampleData;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Infrastructure.Tests;

public class SampleDataReferenceDataProviderTests
{
    private readonly SampleDataReferenceDataProvider _sut = new(new SampleDataProvider());

    [Fact]
    public void GetReferenceData_returns_non_empty_dropdown_sources()
    {
        var data = _sut.GetReferenceData();

        data.Subscriptions.Should().NotBeEmpty();
        data.Clusters.Should().NotBeEmpty();
        data.Namespaces.Should().NotBeEmpty();
        data.Environments.Should().NotBeEmpty();
        data.Tenants.Should().NotBeEmpty();
        data.Applications.Should().NotBeEmpty();
        data.SessionYears.Should().NotBeEmpty();
        data.RuleSets.Should().NotBeEmpty();
        data.DependencyTypes.Should().HaveCount(6);
    }
}
