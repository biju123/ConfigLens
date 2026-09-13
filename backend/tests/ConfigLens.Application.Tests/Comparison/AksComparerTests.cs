using ConfigLens.Application.Comparison;
using ConfigLens.Domain.Scan.Aks;
using FluentAssertions;
using Xunit;

namespace ConfigLens.Application.Tests.Comparison;

public class AksComparerTests
{
    private static AksResource Deployment(string ns, string name, int desired, int running) => new()
    {
        Namespace = ns,
        AppOrService = name,
        ResourceType = AksResourceType.Deployment,
        ResourceName = name,
        DesiredReplicas = desired,
        RunningReplicas = running
    };

    [Fact]
    public void Detects_added_and_removed_resources()
    {
        var current = new[] { Deployment("ns1", "api", 3, 3) };
        var baseline = new[] { Deployment("ns1", "worker", 2, 2) };

        var result = AksComparer.Compare(current, baseline);

        result.AddedResources.Should().ContainSingle(r => r.ResourceName == "api");
        result.RemovedResources.Should().ContainSingle(r => r.ResourceName == "worker");
    }

    [Fact]
    public void Detects_replica_count_changes_on_matching_resources()
    {
        var current = new[] { Deployment("ns1", "api", 3, 2) };
        var baseline = new[] { Deployment("ns1", "api", 3, 3) };

        var result = AksComparer.Compare(current, baseline);

        result.ChangedResources.Should().ContainSingle();
        result.ChangedResources[0].FieldChanges.Should().ContainSingle(f =>
            f.FieldName == nameof(AksResource.RunningReplicas) && f.OldValue == "3" && f.NewValue == "2");
    }

    [Fact]
    public void Reports_no_changes_for_identical_resources()
    {
        var current = new[] { Deployment("ns1", "api", 3, 3) };
        var baseline = new[] { Deployment("ns1", "api", 3, 3) };

        var result = AksComparer.Compare(current, baseline);

        result.AddedResources.Should().BeEmpty();
        result.RemovedResources.Should().BeEmpty();
        result.ChangedResources.Should().BeEmpty();
    }
}
