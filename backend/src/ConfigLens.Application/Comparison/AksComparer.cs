using ConfigLens.Domain.Comparison;
using ConfigLens.Domain.Scan.Aks;

namespace ConfigLens.Application.Comparison;

/// <summary>
/// Matches resources by (Namespace, ResourceType, ResourceName) so CronJobs,
/// HPA, KEDA ScaledObjects and Services are compared the same way as
/// Deployments/Pods - the "sections" CLAUDE.md section 14 lists fall out of
/// grouping the flat ChangedResources list by ResourceType client-side,
/// rather than needing separate comparison code per resource type.
/// </summary>
public static class AksComparer
{
    public static AksComparisonResult Compare(IReadOnlyList<AksResource> current, IReadOnlyList<AksResource> baseline)
    {
        var currentByKey = current.ToDictionary(Key);
        var baselineByKey = baseline.ToDictionary(Key);

        var added = current.Where(r => !baselineByKey.ContainsKey(Key(r))).ToList();
        var removed = baseline.Where(r => !currentByKey.ContainsKey(Key(r))).ToList();

        var changed = new List<AksResourceChange>();
        foreach (var (key, currentResource) in currentByKey)
        {
            if (!baselineByKey.TryGetValue(key, out var baselineResource))
            {
                continue;
            }

            var fieldChanges = DiffFields(currentResource, baselineResource);
            if (fieldChanges.Count > 0)
            {
                changed.Add(new AksResourceChange(currentResource.Namespace, currentResource.ResourceType, currentResource.ResourceName, fieldChanges));
            }
        }

        return new AksComparisonResult(added, removed, changed);
    }

    private static (string Namespace, AksResourceType ResourceType, string ResourceName) Key(AksResource r) =>
        (r.Namespace, r.ResourceType, r.ResourceName);

    private static List<AksFieldChange> DiffFields(AksResource current, AksResource baseline)
    {
        var changes = new List<AksFieldChange>();

        void Add(string name, object? currentValue, object? baselineValue)
        {
            var currentText = currentValue?.ToString();
            var baselineText = baselineValue?.ToString();
            if (!string.Equals(currentText, baselineText, StringComparison.Ordinal))
            {
                changes.Add(new AksFieldChange(name, baselineText, currentText));
            }
        }

        Add(nameof(AksResource.DesiredReplicas), current.DesiredReplicas, baseline.DesiredReplicas);
        Add(nameof(AksResource.RunningReplicas), current.RunningReplicas, baseline.RunningReplicas);
        Add(nameof(AksResource.ReadyReplicas), current.ReadyReplicas, baseline.ReadyReplicas);
        Add(nameof(AksResource.CpuRequest), current.CpuRequest, baseline.CpuRequest);
        Add(nameof(AksResource.CpuLimit), current.CpuLimit, baseline.CpuLimit);
        Add(nameof(AksResource.MemoryRequest), current.MemoryRequest, baseline.MemoryRequest);
        Add(nameof(AksResource.MemoryLimit), current.MemoryLimit, baseline.MemoryLimit);
        Add(nameof(AksResource.Image), current.Image, baseline.Image);
        Add(nameof(AksResource.PodStatus), current.PodStatus, baseline.PodStatus);
        Add(nameof(AksResource.RestartCount), current.RestartCount, baseline.RestartCount);
        Add(nameof(AksResource.Schedule), current.Schedule, baseline.Schedule);
        Add(nameof(AksResource.MinReplicas), current.MinReplicas, baseline.MinReplicas);
        Add(nameof(AksResource.MaxReplicas), current.MaxReplicas, baseline.MaxReplicas);
        Add(nameof(AksResource.CurrentMetricValue), current.CurrentMetricValue, baseline.CurrentMetricValue);
        Add(nameof(AksResource.ClusterIp), current.ClusterIp, baseline.ClusterIp);
        Add(nameof(AksResource.Ports), current.Ports, baseline.Ports);

        return changes;
    }
}
