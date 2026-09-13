using ConfigLens.Application.Services;
using ConfigLens.Domain.Configuration;
using ConfigLens.Domain.Scan;

namespace ConfigLens.Application.Validation;

/// <summary>
/// Everything a rule evaluator needs: the tree being validated (null if no
/// configuration was published at all - a legitimate "missing configuration"
/// scenario, see CLAUDE.md section 18) plus enough scan context to fetch
/// sibling applications'/environments' configuration for the two
/// cross-consistency rule types.
/// </summary>
public sealed record RuleEvaluationContext(
    ScanId ScanId,
    string Application,
    string Environment,
    string Tenant,
    string? Namespace,
    int SessionYear,
    ConfigTree? Tree,
    IApplicationConfigurationClient ConfigurationClient,
    IMaskingService MaskingService)
{
    /// <summary>Renders a config item's value for inclusion in Finding text, masking it if the key looks sensitive so findings never leak secrets.</summary>
    public string DisplayValue(ConfigItem item) => MaskingService.IsSensitiveKey(item.Key) ? "********" : item.Value ?? "(null)";
}
