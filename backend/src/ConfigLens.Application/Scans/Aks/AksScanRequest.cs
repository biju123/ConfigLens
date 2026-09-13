namespace ConfigLens.Application.Scans.Aks;

public sealed record AksScanRequest(
    string SubscriptionId,
    string ClusterName,
    string Environment,
    string Tenant,
    string? Namespace);
