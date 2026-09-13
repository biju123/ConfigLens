using ConfigLens.Domain.Reference;
using ConfigLens.SampleData;

namespace ConfigLens.Infrastructure.ApplicationApis;

public sealed class SampleDataReferenceDataProvider(ISampleDataProvider sampleData) : IReferenceDataProvider
{
    public ReferenceData GetReferenceData()
    {
        var raw = sampleData.GetReferenceData();
        return new ReferenceData(
            raw.Subscriptions,
            raw.Clusters,
            raw.Namespaces,
            raw.Environments,
            raw.Tenants,
            raw.Applications,
            raw.SessionYears,
            raw.RuleSets.Select(r => new RuleSetSummary(r.RuleSetId, r.Name)).ToList(),
            raw.DependencyTypes);
    }
}
