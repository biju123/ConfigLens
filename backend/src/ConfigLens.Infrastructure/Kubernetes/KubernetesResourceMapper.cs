using System.Text.Json;
using ConfigLens.Domain.Scan.Aks;
using k8s.Models;

namespace ConfigLens.Infrastructure.Kubernetes;

/// <summary>
/// Maps raw Kubernetes/AKS API objects into the domain AksResource shape.
/// Pure and I/O-free (unlike AzureKubernetesInventoryReader, which fetches
/// the objects this class maps) so it is directly unit-testable.
/// </summary>
public static class KubernetesResourceMapper
{
    private const string AppLabelKey = "app.kubernetes.io/name";
    private const string LegacyAppLabelKey = "app";

    public static AksResource MapDeployment(V1Deployment resource)
    {
        var container = FirstContainer(resource.Spec?.Template?.Spec);
        return new AksResource
        {
            Namespace = resource.Metadata.NamespaceProperty,
            AppOrService = AppOrServiceName(resource.Metadata),
            ResourceType = AksResourceType.Deployment,
            ResourceName = resource.Metadata.Name,
            DesiredReplicas = resource.Spec?.Replicas,
            RunningReplicas = resource.Status?.Replicas,
            ReadyReplicas = resource.Status?.ReadyReplicas,
            CpuRequest = Quantity(container, "cpu", isRequest: true),
            CpuLimit = Quantity(container, "cpu", isRequest: false),
            MemoryRequest = Quantity(container, "memory", isRequest: true),
            MemoryLimit = Quantity(container, "memory", isRequest: false),
            ContainerCount = resource.Spec?.Template?.Spec?.Containers?.Count,
            Image = container?.Image
        };
    }

    public static AksResource MapPod(V1Pod resource)
    {
        var container = FirstContainer(resource.Spec);
        return new AksResource
        {
            Namespace = resource.Metadata.NamespaceProperty,
            AppOrService = AppOrServiceName(resource.Metadata),
            ResourceType = AksResourceType.Pod,
            ResourceName = resource.Metadata.Name,
            CpuRequest = Quantity(container, "cpu", isRequest: true),
            CpuLimit = Quantity(container, "cpu", isRequest: false),
            MemoryRequest = Quantity(container, "memory", isRequest: true),
            MemoryLimit = Quantity(container, "memory", isRequest: false),
            ContainerCount = resource.Spec?.Containers?.Count,
            Image = container?.Image,
            PodStatus = resource.Status?.Phase,
            RestartCount = resource.Status?.ContainerStatuses?.Sum(cs => cs.RestartCount)
        };
    }

    public static AksResource MapCronJob(V1CronJob resource)
    {
        var container = FirstContainer(resource.Spec?.JobTemplate?.Spec?.Template?.Spec);
        return new AksResource
        {
            Namespace = resource.Metadata.NamespaceProperty,
            AppOrService = AppOrServiceName(resource.Metadata),
            ResourceType = AksResourceType.CronJob,
            ResourceName = resource.Metadata.Name,
            ContainerCount = resource.Spec?.JobTemplate?.Spec?.Template?.Spec?.Containers?.Count,
            Image = container?.Image,
            Schedule = resource.Spec?.Schedule,
            LastScheduleTimeUtc = resource.Status?.LastScheduleTime
        };
    }

    public static AksResource MapJob(V1Job resource)
    {
        var container = FirstContainer(resource.Spec?.Template?.Spec);
        return new AksResource
        {
            Namespace = resource.Metadata.NamespaceProperty,
            AppOrService = AppOrServiceName(resource.Metadata),
            ResourceType = AksResourceType.Job,
            ResourceName = resource.Metadata.Name,
            DesiredReplicas = resource.Spec?.Completions,
            RunningReplicas = resource.Status?.Active,
            ReadyReplicas = resource.Status?.Succeeded,
            ContainerCount = resource.Spec?.Template?.Spec?.Containers?.Count,
            Image = container?.Image
        };
    }

    public static AksResource MapService(V1Service resource) => new()
    {
        Namespace = resource.Metadata.NamespaceProperty,
        AppOrService = AppOrServiceName(resource.Metadata),
        ResourceType = AksResourceType.Service,
        ResourceName = resource.Metadata.Name,
        ClusterIp = resource.Spec?.ClusterIP,
        Ports = resource.Spec?.Ports is { Count: > 0 } ports
            ? string.Join(", ", ports.Select(p => $"{p.Port}:{p.TargetPort}/{p.Protocol ?? "TCP"}"))
            : null
    };

    public static AksResource MapHpa(V2HorizontalPodAutoscaler resource) => new()
    {
        Namespace = resource.Metadata.NamespaceProperty,
        AppOrService = resource.Spec?.ScaleTargetRef?.Name ?? AppOrServiceName(resource.Metadata),
        ResourceType = AksResourceType.Hpa,
        ResourceName = resource.Metadata.Name,
        DesiredReplicas = resource.Status?.DesiredReplicas,
        RunningReplicas = resource.Status?.CurrentReplicas,
        MinReplicas = resource.Spec?.MinReplicas,
        MaxReplicas = resource.Spec?.MaxReplicas,
        CurrentMetricValue = resource.Status?.CurrentMetrics is { Count: > 0 } metrics
            ? string.Join(", ", metrics.Select(DescribeMetric))
            : null
    };

    public static AksResource MapScaledObject(JsonElement resource)
    {
        var metadata = resource.TryGetProperty("metadata", out var m) ? m : default;
        var spec = resource.TryGetProperty("spec", out var s) ? s : default;

        var namespaceName = GetString(metadata, "namespace") ?? "";
        var name = GetString(metadata, "name") ?? "";
        var scaleTargetName = spec.ValueKind == JsonValueKind.Object && spec.TryGetProperty("scaleTargetRef", out var target)
            ? GetString(target, "name")
            : null;

        return new AksResource
        {
            Namespace = namespaceName,
            AppOrService = scaleTargetName ?? name,
            ResourceType = AksResourceType.KedaScaledObject,
            ResourceName = name,
            MinReplicas = GetInt(spec, "minReplicaCount"),
            MaxReplicas = GetInt(spec, "maxReplicaCount"),
            CurrentMetricValue = spec.ValueKind == JsonValueKind.Object && spec.TryGetProperty("triggers", out var triggers) && triggers.ValueKind == JsonValueKind.Array
                ? string.Join(", ", triggers.EnumerateArray().Select(t => GetString(t, "type")).Where(t => t is not null))
                : null
        };
    }

    private static V1Container? FirstContainer(V1PodSpec? podSpec) => podSpec?.Containers?.FirstOrDefault();

    private static string AppOrServiceName(V1ObjectMeta metadata)
    {
        if (metadata.Labels is { } labels)
        {
            if (labels.TryGetValue(AppLabelKey, out var name) && !string.IsNullOrWhiteSpace(name)) return name;
            if (labels.TryGetValue(LegacyAppLabelKey, out var legacyName) && !string.IsNullOrWhiteSpace(legacyName)) return legacyName;
        }
        return metadata.Name;
    }

    private static string? Quantity(V1Container? container, string resourceName, bool isRequest)
    {
        var resources = isRequest ? container?.Resources?.Requests : container?.Resources?.Limits;
        return resources is not null && resources.TryGetValue(resourceName, out var quantity) ? quantity.ToString() : null;
    }

    private static string DescribeMetric(V2MetricStatus metric)
    {
        var utilization = metric.Resource?.Current?.AverageUtilization;
        if (utilization is not null)
        {
            return $"{metric.Resource!.Name} {utilization}%";
        }
        return metric.Type;
    }

    private static string? GetString(JsonElement element, string propertyName) =>
        element.ValueKind == JsonValueKind.Object && element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()
            : null;

    private static int? GetInt(JsonElement element, string propertyName) =>
        element.ValueKind == JsonValueKind.Object && element.TryGetProperty(propertyName, out var value) && value.ValueKind == JsonValueKind.Number
            ? value.GetInt32()
            : null;
}
