using ConfigLens.Domain.Scan.Aks;

namespace ConfigLens.Domain.Comparison;

public sealed record AksFieldChange(string FieldName, string? OldValue, string? NewValue);

public sealed record AksResourceChange(
    string Namespace,
    AksResourceType ResourceType,
    string ResourceName,
    IReadOnlyList<AksFieldChange> FieldChanges);

/// <summary>
/// Covers resources, replica counts, CPU, memory, images, CronJobs, HPA,
/// KEDA and Services uniformly - AksResource.ResourceType is part of the
/// match key, so category-specific "sections" fall out of grouping by it
/// rather than needing separate comparison logic per resource type.
/// </summary>
public sealed record AksComparisonResult(
    IReadOnlyList<AksResource> AddedResources,
    IReadOnlyList<AksResource> RemovedResources,
    IReadOnlyList<AksResourceChange> ChangedResources);
