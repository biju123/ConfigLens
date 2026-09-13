using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Findings;
using ConfigLens.SampleData;
using ConfigLens.SampleData.Raw;

namespace ConfigLens.Infrastructure.ApplicationApis;

public sealed class SampleDataRuleSetProvider(ISampleDataProvider sampleData) : IRuleSetProvider
{
    public RuleSet? GetRuleSet(string ruleSetId)
    {
        var raw = sampleData.GetRuleSets().FirstOrDefault(r => string.Equals(r.RuleSetId, ruleSetId, StringComparison.OrdinalIgnoreCase));
        return raw is null ? null : Map(raw);
    }

    public IReadOnlyList<RuleSet> GetAllRuleSets() => sampleData.GetRuleSets().Select(Map).ToList();

    private static RuleSet Map(RawRuleSet raw) => new(raw.RuleSetId, raw.Name, raw.Rules.Select(MapRule).ToList());

    private static ValidationRule MapRule(RawValidationRule raw) => new()
    {
        RuleId = raw.RuleId,
        RuleName = raw.RuleName,
        RuleType = Enum.Parse<RuleType>(raw.RuleType, ignoreCase: true),
        TargetPath = raw.TargetPath,
        Severity = Enum.Parse<FindingSeverity>(raw.Severity, ignoreCase: true),
        Description = raw.Description,
        Recommendation = raw.Recommendation,
        Parameters = raw.Parameters
    };
}
