using ConfigLens.Domain.Scan.Aks;
using ConfigLens.SampleData;
using ConfigLens.SampleData.Raw;

namespace ConfigLens.Infrastructure.Kubernetes;

public sealed class SampleDataKubernetesInventoryReader(ISampleDataProvider sampleData) : IKubernetesInventoryReader
{
    public IReadOnlyList<AksResource> GetResources(AksInventoryQuery query)
    {
        return sampleData.GetAksResources()
            .Where(r => string.Equals(r.SubscriptionId, query.SubscriptionId, StringComparison.OrdinalIgnoreCase))
            .Where(r => string.Equals(r.ClusterName, query.ClusterName, StringComparison.OrdinalIgnoreCase))
            .Where(r => string.Equals(r.Environment, query.Environment, StringComparison.OrdinalIgnoreCase))
            .Where(r => string.Equals(r.Tenant, query.Tenant, StringComparison.OrdinalIgnoreCase))
            .Where(r => query.Namespace is null || string.Equals(r.Namespace, query.Namespace, StringComparison.OrdinalIgnoreCase))
            .Select(Map)
            .ToList();
    }

    private static AksResource Map(RawAksResource raw) => new()
    {
        Namespace = raw.Namespace,
        AppOrService = raw.AppOrService,
        ResourceType = Enum.Parse<AksResourceType>(raw.ResourceType, ignoreCase: true),
        ResourceName = raw.ResourceName,
        DesiredReplicas = raw.DesiredReplicas,
        RunningReplicas = raw.RunningReplicas,
        ReadyReplicas = raw.ReadyReplicas,
        CpuRequest = raw.CpuRequest,
        CpuLimit = raw.CpuLimit,
        MemoryRequest = raw.MemoryRequest,
        MemoryLimit = raw.MemoryLimit,
        ContainerCount = raw.ContainerCount,
        Image = raw.Image,
        PodStatus = raw.PodStatus,
        RestartCount = raw.RestartCount,
        Schedule = raw.Schedule,
        LastScheduleTimeUtc = raw.LastScheduleTimeUtc,
        MinReplicas = raw.MinReplicas,
        MaxReplicas = raw.MaxReplicas,
        CurrentMetricValue = raw.CurrentMetricValue,
        ClusterIp = raw.ClusterIp,
        Ports = raw.Ports
    };
}
