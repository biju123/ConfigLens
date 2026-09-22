using Azure;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.ContainerService;
using Azure.ResourceManager.Resources;
using ConfigLens.Domain.Scan.Aks;
using k8s;
using k8s.Autorest;

namespace ConfigLens.Infrastructure.Kubernetes;

/// <summary>
/// Resolves an AKS managed cluster by subscription + cluster name via Azure
/// Resource Manager and builds a Kubernetes API client from its Azure-issued
/// user credentials. Shared by AzureAksClusterDirectory (cluster/namespace
/// discovery) and AzureKubernetesInventoryReader (the real AKS scan) so both
/// authenticate against Azure/AKS the same way.
/// </summary>
public sealed class AksClusterConnector(ArmClient armClient)
{
    public IReadOnlyList<string> ListClusterNames(string subscriptionId)
    {
        var subscription = GetSubscription(subscriptionId);

        return RunOrThrow(
            () => subscription.GetContainerServiceManagedClusters()
                .Select(c => c.Data.Name)
                .OrderBy(name => name, StringComparer.OrdinalIgnoreCase)
                .ToList(),
            $"Unable to list AKS clusters in subscription '{subscriptionId}'.");
    }

    public IKubernetes Connect(string subscriptionId, string clusterName)
    {
        var subscription = GetSubscription(subscriptionId);

        var cluster = RunOrThrow(
            () => subscription.GetContainerServiceManagedClusters()
                .FirstOrDefault(c => string.Equals(c.Data.Name, clusterName, StringComparison.OrdinalIgnoreCase)),
            $"Unable to list AKS clusters in subscription '{subscriptionId}'.");

        if (cluster is null)
        {
            throw new AksClusterNotFoundException($"AKS cluster '{clusterName}' was not found in subscription '{subscriptionId}'.");
        }

        var kubeconfigBytes = RunOrThrow(
            () => cluster.GetClusterUserCredentials().Value.Kubeconfigs.First().Value,
            $"Unable to obtain credentials for AKS cluster '{clusterName}'.");

        using var kubeconfigStream = new MemoryStream(kubeconfigBytes);
        var config = KubernetesClientConfiguration.BuildConfigFromConfigFile(kubeconfigStream);
        return new k8s.Kubernetes(config);
    }

    private SubscriptionResource GetSubscription(string subscriptionId)
    {
        try
        {
            return armClient.GetSubscriptions().Get(subscriptionId);
        }
        catch (RequestFailedException ex) when (ex.Status == 404)
        {
            throw new AksClusterNotFoundException($"Azure subscription '{subscriptionId}' was not found or is not accessible.", ex);
        }
        catch (RequestFailedException ex)
        {
            throw new AksConnectivityException($"Unable to reach Azure Resource Manager for subscription '{subscriptionId}'.", ex);
        }
        catch (AuthenticationFailedException ex)
        {
            // Covers CredentialUnavailableException too (it derives from this) - no usable Azure
            // credential (az login / managed identity / AZURE_* env vars) was found, per CLAUDE.md
            // section 19's "appropriate authentication mechanisms". Surfaced as infrastructure-failure
            // rather than a raw 500 so the UI can explain it's an environment/setup problem.
            throw new AksConnectivityException("Unable to authenticate to Azure. Check the backend's Azure credentials (az login, managed identity, or AZURE_* environment variables).", ex);
        }
    }

    /// <summary>
    /// Runs an Azure Resource Manager or Kubernetes API call, translating
    /// transport/auth failures into the typed exceptions ConfigLensExceptionHandler
    /// maps to the infrastructure-failure/timeout error categories (CLAUDE.md section 23).
    /// </summary>
    public static T RunOrThrow<T>(Func<T> operation, string contextMessage)
    {
        try
        {
            return operation();
        }
        catch (RequestFailedException ex)
        {
            throw new AksConnectivityException(contextMessage, ex);
        }
        catch (AuthenticationFailedException ex)
        {
            throw new AksConnectivityException(contextMessage, ex);
        }
        catch (HttpOperationException ex)
        {
            throw new AksConnectivityException(contextMessage, ex);
        }
        catch (HttpRequestException ex)
        {
            throw new AksConnectivityException(contextMessage, ex);
        }
        catch (OperationCanceledException ex)
        {
            throw new AksTimeoutException(contextMessage, ex);
        }
    }
}
