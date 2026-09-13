namespace ConfigLens.Domain.Scan.Aks;

public enum AksResourceType
{
    Deployment,
    Pod,
    CronJob,
    Job,
    Service,
    Hpa,
    KedaScaledObject
}
