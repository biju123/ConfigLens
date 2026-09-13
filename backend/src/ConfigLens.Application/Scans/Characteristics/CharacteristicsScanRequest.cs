namespace ConfigLens.Application.Scans.Characteristics;

public sealed record CharacteristicsScanRequest(
    string Application,
    string Environment,
    string Tenant,
    int SessionYear,
    string RuleSetId);
