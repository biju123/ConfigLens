using ConfigLens.Application.Scans.Aks;
using ConfigLens.Application.Services;
using ConfigLens.Application.Tests.Fakes;
using ConfigLens.Domain.Scan;
using ConfigLens.Domain.Scan.Aks;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace ConfigLens.Application.Tests.Scans;

public class AksDeploymentScanServiceTests
{
    [Fact]
    public void Execute_returns_resources_and_stores_the_scan_in_the_repository()
    {
        var resources = new[] { new AksResource { Namespace = "assessor-prod", AppOrService = "assessor-api", ResourceType = AksResourceType.Deployment, ResourceName = "assessor-api" } };
        var reader = new FakeKubernetesInventoryReader(resources);
        var repository = new InMemoryScanRepository();
        var sut = new AksDeploymentScanService(reader, new ScanIdGenerator(new FakeClock(DateTime.UtcNow)), repository, new FakeClock(DateTime.UtcNow), NullLogger<AksDeploymentScanService>.Instance);

        var request = new AksScanRequest("sub-1", "cluster-1", "Production", "ContosoCounty", "assessor-prod");
        var response = sut.Execute(request, "user");

        response.Resources.Should().BeEquivalentTo(resources);
        response.Metadata.Status.Should().Be(ScanStatus.Completed);
        repository.GetById(response.Metadata.ScanId).Should().NotBeNull();
        reader.LastQuery.Should().Be(new AksInventoryQuery("sub-1", "cluster-1", "Production", "ContosoCounty", "assessor-prod"));
    }
}
