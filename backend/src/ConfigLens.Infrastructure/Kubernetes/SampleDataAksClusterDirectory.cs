using ConfigLens.Domain.Scan.Aks;
using ConfigLens.SampleData;

namespace ConfigLens.Infrastructure.Kubernetes;

/// <summary>
/// Derives cluster/namespace discovery results from the same sample AKS
/// inventory used by SampleDataKubernetesInventoryReader, so the two stay
/// consistent for local/offline use and tests.
/// </summary>
public sealed class SampleDataAksClusterDirectory(ISampleDataProvider sampleData) : IAksClusterDirectory
{
    public IReadOnlyList<string> GetClusters(string subscriptionId) =>
        sampleData.GetAksResources()
            .Where(r => string.Equals(r.SubscriptionId, subscriptionId, StringComparison.OrdinalIgnoreCase))
            .Select(r => r.ClusterName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();

    public IReadOnlyList<string> GetNamespaces(string subscriptionId, string clusterName) =>
        sampleData.GetAksResources()
            .Where(r => string.Equals(r.SubscriptionId, subscriptionId, StringComparison.OrdinalIgnoreCase))
            .Where(r => string.Equals(r.ClusterName, clusterName, StringComparison.OrdinalIgnoreCase))
            .Select(r => r.Namespace)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
            .ToList();
}
