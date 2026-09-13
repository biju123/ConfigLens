namespace ConfigLens.Domain.Scan.Aks;

/// <summary>
/// A single inspected Kubernetes resource. Not every field applies to every
/// ResourceType (e.g. a Service has no replica counts, a CronJob has no CPU
/// request) - inapplicable fields are simply left null rather than forcing
/// every resource type into one fully-populated shape.
/// </summary>
public sealed record AksResource
{
    public required string Namespace { get; init; }
    public required string AppOrService { get; init; }
    public required AksResourceType ResourceType { get; init; }
    public required string ResourceName { get; init; }

    public int? DesiredReplicas { get; init; }
    public int? RunningReplicas { get; init; }
    public int? ReadyReplicas { get; init; }

    public string? CpuRequest { get; init; }
    public string? CpuLimit { get; init; }
    public string? MemoryRequest { get; init; }
    public string? MemoryLimit { get; init; }

    public int? ContainerCount { get; init; }
    public string? Image { get; init; }
    public string? PodStatus { get; init; }
    public int? RestartCount { get; init; }

    /// <summary>CronJob schedule expression, e.g. "0 */6 * * *".</summary>
    public string? Schedule { get; init; }
    public DateTime? LastScheduleTimeUtc { get; init; }

    /// <summary>HPA/KEDA ScaledObject bounds.</summary>
    public int? MinReplicas { get; init; }
    public int? MaxReplicas { get; init; }
    public string? CurrentMetricValue { get; init; }

    /// <summary>Service-only fields.</summary>
    public string? ClusterIp { get; init; }
    public string? Ports { get; init; }
}
