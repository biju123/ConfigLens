using ConfigLens.Domain.Comparison;
using ConfigLens.Domain.Findings;

namespace ConfigLens.Application.Comparison;

public static class CharacteristicsComparer
{
    public static CharacteristicsComparisonResult Compare(IReadOnlyList<Finding> current, IReadOnlyList<Finding> baseline)
    {
        var currentByKey = current.ToDictionary(Key);
        var baselineByKey = baseline.ToDictionary(Key);

        var newFailures = current.Where(f => f.Status == FindingStatus.Fail &&
            (!baselineByKey.TryGetValue(Key(f), out var b) || b.Status != FindingStatus.Fail)).ToList();

        var resolvedFailures = baseline.Where(f => f.Status == FindingStatus.Fail &&
            (!currentByKey.TryGetValue(Key(f), out var c) || c.Status != FindingStatus.Fail)).ToList();

        var newWarnings = current.Where(f => f.Status == FindingStatus.Warning &&
            (!baselineByKey.TryGetValue(Key(f), out var b) || b.Status != FindingStatus.Warning)).ToList();

        var resolvedWarnings = baseline.Where(f => f.Status == FindingStatus.Warning &&
            (!currentByKey.TryGetValue(Key(f), out var c) || c.Status != FindingStatus.Warning)).ToList();

        var statusChanges = new List<FindingStatusChange>();
        foreach (var (key, currentFinding) in currentByKey)
        {
            if (baselineByKey.TryGetValue(key, out var baselineFinding) && baselineFinding.Status != currentFinding.Status)
            {
                statusChanges.Add(new FindingStatusChange(currentFinding.RuleId, currentFinding.Resource, baselineFinding.Status, currentFinding.Status));
            }
        }

        var ruleChanges = ComputeRuleChanges(current, baseline);

        return new CharacteristicsComparisonResult(newFailures, resolvedFailures, newWarnings, resolvedWarnings, statusChanges, ruleChanges);
    }

    private static (string RuleId, string Resource) Key(Finding f) => (f.RuleId, f.Resource);

    private static List<RuleChange> ComputeRuleChanges(IReadOnlyList<Finding> current, IReadOnlyList<Finding> baseline)
    {
        var currentRules = current.GroupBy(f => f.RuleId).ToDictionary(g => g.Key, g => g.First());
        var baselineRules = baseline.GroupBy(f => f.RuleId).ToDictionary(g => g.Key, g => g.First());

        var changes = new List<RuleChange>();

        foreach (var (ruleId, finding) in currentRules)
        {
            if (!baselineRules.TryGetValue(ruleId, out var baselineFinding))
            {
                changes.Add(new RuleChange(ruleId, finding.RuleName, RuleChangeType.Added));
            }
            else if (baselineFinding.RuleName != finding.RuleName || baselineFinding.Severity != finding.Severity)
            {
                changes.Add(new RuleChange(ruleId, finding.RuleName, RuleChangeType.Modified));
            }
        }

        foreach (var (ruleId, finding) in baselineRules)
        {
            if (!currentRules.ContainsKey(ruleId))
            {
                changes.Add(new RuleChange(ruleId, finding.RuleName, RuleChangeType.Removed));
            }
        }

        return changes;
    }
}
