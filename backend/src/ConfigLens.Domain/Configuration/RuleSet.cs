namespace ConfigLens.Domain.Configuration;

public sealed record RuleSet(string RuleSetId, string Name, IReadOnlyList<ValidationRule> Rules);
