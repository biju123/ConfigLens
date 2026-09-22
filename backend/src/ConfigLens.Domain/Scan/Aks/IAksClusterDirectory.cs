namespace ConfigLens.Domain.Scan.Aks;

/// <summary>
/// Port for discovering AKS clusters within an Azure subscription and
/// namespaces within a cluster - backs the scan-parameter dropdowns
/// (CLAUDE.md section 7). The MVP implementation
/// (Infrastructure.Kubernetes.AzureAksClusterDirectory) calls Azure Resource
/// Manager and the cluster's own API server; a sample-data-backed
/// implementation exists for local/offline use and tests.
/// </summary>
public interface IAksClusterDirectory
{
    IReadOnlyList<string> GetClusters(string subscriptionId);

    IReadOnlyList<string> GetNamespaces(string subscriptionId, string clusterName);
}
