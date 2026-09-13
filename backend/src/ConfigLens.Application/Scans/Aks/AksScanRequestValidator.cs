using FluentValidation;

namespace ConfigLens.Application.Scans.Aks;

public sealed class AksScanRequestValidator : AbstractValidator<AksScanRequest>
{
    public AksScanRequestValidator()
    {
        RuleFor(x => x.SubscriptionId).NotEmpty();
        RuleFor(x => x.ClusterName).NotEmpty();
        RuleFor(x => x.Environment).NotEmpty();
        RuleFor(x => x.Tenant).NotEmpty();
    }
}
