using ConfigLens.Domain.Configuration;

namespace ConfigLens.Application.Tests.Fakes;

public sealed class FakeRuleSetProvider : IRuleSetProvider
{
    private readonly Dictionary<string, RuleSet> _ruleSets = [];

    public FakeRuleSetProvider With(RuleSet ruleSet)
    {
        _ruleSets[ruleSet.RuleSetId] = ruleSet;
        return this;
    }

    public RuleSet? GetRuleSet(string ruleSetId) => _ruleSets.GetValueOrDefault(ruleSetId);

    public IReadOnlyList<RuleSet> GetAllRuleSets() => _ruleSets.Values.ToList();
}
