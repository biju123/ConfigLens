using ConfigLens.Domain.Configuration;
using ConfigLens.Infrastructure.ApplicationApis;
using ConfigLens.SampleData;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Infrastructure.Tests;

public class SampleDataApplicationConfigurationClientTests
{
    private readonly SampleDataApplicationConfigurationClient _sut = new(new SampleDataProvider());

    [Fact]
    public void GetConfiguration_returns_the_matching_tree_with_unmasked_values()
    {
        var tree = _sut.GetConfiguration(new ApplicationConfigurationQuery("AssessorApi", "Production", "ContosoCounty", "assessor-prod", 2026));

        tree.Should().NotBeNull();
        var database = tree!.Sections.Single(s => s.Name == "Database");
        database.Items.Single(i => i.Key == "ConnectionString").Value.Should().Contain("Password=");
        database.Items.Single(i => i.Key == "ConnectionString").IsSensitive.Should().BeTrue();
    }

    [Fact]
    public void GetConfiguration_returns_null_when_no_configuration_is_published_for_the_session_year()
    {
        var tree = _sut.GetConfiguration(new ApplicationConfigurationQuery("AssessorApi", "Production", "ContosoCounty", "assessor-prod", 2024));

        tree.Should().BeNull();
    }
}
