using FluentValidation;

namespace ConfigLens.Application.Scans.AppConfig;

public sealed class AppConfigScanRequestValidator : AbstractValidator<AppConfigScanRequest>
{
    public AppConfigScanRequestValidator()
    {
        RuleFor(x => x.Application).NotEmpty();
        RuleFor(x => x.Environment).NotEmpty();
        RuleFor(x => x.Tenant).NotEmpty();
        RuleFor(x => x.SessionYear).InclusiveBetween(2000, 2100);
    }
}
