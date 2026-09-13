namespace ConfigLens.Domain.Configuration;

public enum RuleType
{
    RequiredExists,
    NotEmpty,
    FormatMatch,
    AllowedValues,
    NumericRange,
    SessionYearExists,
    CrossApplicationConsistency,
    CrossEnvironmentConsistency
}
