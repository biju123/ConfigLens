namespace ConfigLens.Domain.Configuration;

public interface IRuleSetProvider
{
    RuleSet? GetRuleSet(string ruleSetId);

    IReadOnlyList<RuleSet> GetAllRuleSets();
}
