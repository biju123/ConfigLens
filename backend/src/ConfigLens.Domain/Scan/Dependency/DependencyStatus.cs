namespace ConfigLens.Domain.Scan.Dependency;

public enum DependencyStatus
{
    ConfigurationMissing,
    ConfigurationInvalid,
    Inaccessible,
    Accessible,
    Timeout,
    ValidationError,
    NotApplicable
}
