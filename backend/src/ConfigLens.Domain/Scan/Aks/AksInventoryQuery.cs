namespace ConfigLens.Domain.Scan.Aks;

public sealed record AksInventoryQuery(
    string SubscriptionId,
    string ClusterName,
    string Environment,
    string Tenant,
    string? Namespace);
