namespace ConfigLens.Domain.Scan.Aks;

public sealed record AksScanResult(IReadOnlyList<AksResource> Resources) : IScanResult
{
    public ScanCategory Category => ScanCategory.AksDeployment;
}
