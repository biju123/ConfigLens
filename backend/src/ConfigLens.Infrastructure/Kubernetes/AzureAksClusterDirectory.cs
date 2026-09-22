using ConfigLens.Domain.Scan.Aks;
using k8s;

namespace ConfigLens.Infrastructure.Kubernetes;

/// <summary>Real AKS cluster/namespace discovery via Azure Resource Manager and the cluster's own API server.</summary>
public sealed class AzureAksClusterDirectory(AksClusterConnector connector) : IAksClusterDirectory
{
    public IReadOnlyList<string> GetClusters(string subscriptionId) =>
        connector.ListClusterNames(subscriptionId);

    public IReadOnlyList<string> GetNamespaces(string subscriptionId, string clusterName)
    {
        var client = connector.Connect(subscriptionId, clusterName);

        return AksClusterConnector.RunOrThrow(
            () => client.CoreV1.ListNamespace().Items
                .Select(ns => ns.Metadata.Name)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList(),
            $"Unable to list namespaces for AKS cluster '{clusterName}'.");
    }
}
