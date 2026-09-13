using ConfigLens.Domain.Scan.Aks;
using ConfigLens.Infrastructure.Kubernetes;
using ConfigLens.SampleData;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Infrastructure.Tests;

public class SampleDataKubernetesInventoryReaderTests
{
    private readonly SampleDataKubernetesInventoryReader _sut = new(new SampleDataProvider());

    [Fact]
    public void GetResources_filters_by_cluster_environment_tenant_and_namespace()
    {
        var query = new AksInventoryQuery("sub-assessor-prod-01", "aks-assessor-prod-eastus", "Production", "ContosoCounty", "assessor-prod");

        var resources = _sut.GetResources(query);

        resources.Should().NotBeEmpty();
        resources.Should().OnlyContain(r => r.Namespace == "assessor-prod");
    }

    [Fact]
    public void GetResources_covers_every_resource_type_in_the_prod_namespace()
    {
        var query = new AksInventoryQuery("sub-assessor-prod-01", "aks-assessor-prod-eastus", "Production", "ContosoCounty", "assessor-prod");

        var resources = _sut.GetResources(query);

        var types = resources.Select(r => r.ResourceType).ToHashSet();
        types.Should().Contain([
            AksResourceType.Deployment, AksResourceType.Pod, AksResourceType.CronJob,
            AksResourceType.Job, AksResourceType.Service, AksResourceType.Hpa, AksResourceType.KedaScaledObject
        ]);
    }

    [Fact]
    public void GetResources_surfaces_the_under_replica_and_high_restart_scenarios()
    {
        var query = new AksInventoryQuery("sub-assessor-prod-01", "aks-assessor-prod-eastus", "Production", "ContosoCounty", "assessor-prod");

        var resources = _sut.GetResources(query);

        var workerDeployment = resources.Single(r => r.ResourceType == AksResourceType.Deployment && r.AppOrService == "assessor-worker");
        (workerDeployment.RunningReplicas < workerDeployment.DesiredReplicas).Should().BeTrue();

        resources.Should().Contain(r => r.ResourceType == AksResourceType.Pod && r.RestartCount > 5);
    }

    [Fact]
    public void GetResources_without_namespace_filter_returns_all_namespaces_for_the_cluster_and_tenant()
    {
        var query = new AksInventoryQuery("sub-assessor-prod-01", "aks-assessor-prod-eastus", "Production", "RiverbendCounty", null);

        var resources = _sut.GetResources(query);

        resources.Should().OnlyContain(r => r.Namespace == "assessor-prod-riverbend");
    }

    [Fact]
    public void GetResources_returns_empty_for_unknown_cluster()
    {
        var query = new AksInventoryQuery("sub-unknown", "aks-unknown", "Production", "ContosoCounty", null);

        _sut.GetResources(query).Should().BeEmpty();
    }
}
