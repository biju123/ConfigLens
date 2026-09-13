using ConfigLens.Domain.Scan.Aks;

namespace ConfigLens.Application.Tests.Fakes;

public sealed class FakeKubernetesInventoryReader(IReadOnlyList<AksResource> resources) : IKubernetesInventoryReader
{
    public AksInventoryQuery? LastQuery { get; private set; }

    public IReadOnlyList<AksResource> GetResources(AksInventoryQuery query)
    {
        LastQuery = query;
        return resources;
    }
}
