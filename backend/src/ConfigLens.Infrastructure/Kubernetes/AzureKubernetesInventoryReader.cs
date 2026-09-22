using System.Net;
using System.Text.Json;
using ConfigLens.Domain.Scan.Aks;
using k8s;
using k8s.Autorest;
using k8s.Models;

namespace ConfigLens.Infrastructure.Kubernetes;

/// <summary>
/// Real AKS deployment scan: connects to the cluster identified by the scan
/// request's subscription + cluster name and reads its live inventory,
/// filtered to a single namespace when one is given (CLAUDE.md section 8).
/// </summary>
public sealed class AzureKubernetesInventoryReader(AksClusterConnector connector) : IKubernetesInventoryReader
{
    private const string KedaGroup = "keda.sh";
    private const string KedaVersion = "v1alpha1";
    private const string ScaledObjectsPlural = "scaledobjects";

    public IReadOnlyList<AksResource> GetResources(AksInventoryQuery query)
    {
        var client = connector.Connect(query.SubscriptionId, query.ClusterName);
        var ns = string.IsNullOrWhiteSpace(query.Namespace) ? null : query.Namespace;

        return AksClusterConnector.RunOrThrow(() =>
        {
            var resources = new List<AksResource>();
            resources.AddRange(ListDeployments(client, ns).Select(KubernetesResourceMapper.MapDeployment));
            resources.AddRange(ListPods(client, ns).Select(KubernetesResourceMapper.MapPod));
            resources.AddRange(ListCronJobs(client, ns).Select(KubernetesResourceMapper.MapCronJob));
            resources.AddRange(ListJobs(client, ns).Select(KubernetesResourceMapper.MapJob));
            resources.AddRange(ListServices(client, ns).Select(KubernetesResourceMapper.MapService));
            resources.AddRange(ListHpas(client, ns).Select(KubernetesResourceMapper.MapHpa));
            resources.AddRange(ListScaledObjects(client, ns).Select(KubernetesResourceMapper.MapScaledObject));
            return resources;
        }, $"Unable to read AKS inventory for cluster '{query.ClusterName}'.");
    }

    private static IList<V1Deployment> ListDeployments(IKubernetes client, string? ns) =>
        (ns is null ? client.AppsV1.ListDeploymentForAllNamespaces() : client.AppsV1.ListNamespacedDeployment(ns)).Items;

    private static IList<V1Pod> ListPods(IKubernetes client, string? ns) =>
        (ns is null ? client.CoreV1.ListPodForAllNamespaces() : client.CoreV1.ListNamespacedPod(ns)).Items;

    private static IList<V1CronJob> ListCronJobs(IKubernetes client, string? ns) =>
        (ns is null ? client.BatchV1.ListCronJobForAllNamespaces() : client.BatchV1.ListNamespacedCronJob(ns)).Items;

    private static IList<V1Job> ListJobs(IKubernetes client, string? ns) =>
        (ns is null ? client.BatchV1.ListJobForAllNamespaces() : client.BatchV1.ListNamespacedJob(ns)).Items;

    private static IList<V1Service> ListServices(IKubernetes client, string? ns) =>
        (ns is null ? client.CoreV1.ListServiceForAllNamespaces() : client.CoreV1.ListNamespacedService(ns)).Items;

    private static IList<V2HorizontalPodAutoscaler> ListHpas(IKubernetes client, string? ns) =>
        (ns is null ? client.AutoscalingV2.ListHorizontalPodAutoscalerForAllNamespaces() : client.AutoscalingV2.ListNamespacedHorizontalPodAutoscaler(ns)).Items;

    private static List<JsonElement> ListScaledObjects(IKubernetes client, string? ns)
    {
        try
        {
            object result = ns is null
                ? client.CustomObjects.ListClusterCustomObject(KedaGroup, KedaVersion, ScaledObjectsPlural)
                : client.CustomObjects.ListNamespacedCustomObject(KedaGroup, KedaVersion, ns, ScaledObjectsPlural);

            using var document = JsonDocument.Parse(JsonSerializer.Serialize(result));
            return document.RootElement.TryGetProperty("items", out var items) && items.ValueKind == JsonValueKind.Array
                ? items.EnumerateArray().Select(item => item.Clone()).ToList()
                : [];
        }
        catch (HttpOperationException ex) when (ex.Response.StatusCode == HttpStatusCode.NotFound)
        {
            // The KEDA CustomResourceDefinition is not installed on this cluster - no ScaledObjects to report,
            // not a scan failure.
            return [];
        }
    }
}
