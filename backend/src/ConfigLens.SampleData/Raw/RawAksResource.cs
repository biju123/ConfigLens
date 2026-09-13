namespace ConfigLens.SampleData.Raw;

/// <summary>Plain POCO mirroring aks-inventory.json - deliberately has no dependency on ConfigLens.Domain; Infrastructure maps these into Domain.Scan.Aks.AksResource.</summary>
public sealed class RawAksResource
{
    public string SubscriptionId { get; set; } = "";
    public string ClusterName { get; set; } = "";
    public string Environment { get; set; } = "";
    public string Tenant { get; set; } = "";
    public string Namespace { get; set; } = "";
    public string AppOrService { get; set; } = "";
    public string ResourceType { get; set; } = "";
    public string ResourceName { get; set; } = "";

    public int? DesiredReplicas { get; set; }
    public int? RunningReplicas { get; set; }
    public int? ReadyReplicas { get; set; }

    public string? CpuRequest { get; set; }
    public string? CpuLimit { get; set; }
    public string? MemoryRequest { get; set; }
    public string? MemoryLimit { get; set; }

    public int? ContainerCount { get; set; }
    public string? Image { get; set; }
    public string? PodStatus { get; set; }
    public int? RestartCount { get; set; }

    public string? Schedule { get; set; }
    public DateTime? LastScheduleTimeUtc { get; set; }

    public int? MinReplicas { get; set; }
    public int? MaxReplicas { get; set; }
    public string? CurrentMetricValue { get; set; }

    public string? ClusterIp { get; set; }
    public string? Ports { get; set; }
}
