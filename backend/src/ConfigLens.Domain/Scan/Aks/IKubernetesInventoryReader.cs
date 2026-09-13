namespace ConfigLens.Domain.Scan.Aks;

/// <summary>
/// Port for reading Kubernetes/AKS inventory. The MVP implementation
/// (Infrastructure.Kubernetes.SampleDataKubernetesInventoryReader) reads
/// deterministic sample data; a later implementation can call the real AKS
/// API without this interface - or any Application code - changing.
/// </summary>
public interface IKubernetesInventoryReader
{
    IReadOnlyList<AksResource> GetResources(AksInventoryQuery query);
}
