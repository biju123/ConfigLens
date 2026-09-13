using ConfigLens.SampleData.Raw;

namespace ConfigLens.SampleData;

/// <summary>
/// Loads the deterministic dummy JSON data used by the MVP. Infrastructure
/// implementations depend on this instead of touching real Azure/Kubernetes
/// SDKs; swapping in real integrations later means replacing the
/// Infrastructure readers, not this provider.
/// </summary>
public interface ISampleDataProvider
{
    IReadOnlyList<RawAksResource> GetAksResources();
    IReadOnlyList<RawAppConfigEntry> GetAppConfigCatalog();
    IReadOnlyList<RawRuleSet> GetRuleSets();
    IReadOnlyList<RawDependencyScenario> GetDependencyScenarios();
    RawReferenceData GetReferenceData();
}
