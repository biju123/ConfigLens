using FluentValidation;

namespace ConfigLens.Application.Scans.Characteristics;

public sealed class CharacteristicsScanRequestValidator : AbstractValidator<CharacteristicsScanRequest>
{
    public CharacteristicsScanRequestValidator()
    {
        RuleFor(x => x.Application).NotEmpty();
        RuleFor(x => x.Environment).NotEmpty();
        RuleFor(x => x.Tenant).NotEmpty();
        RuleFor(x => x.SessionYear).InclusiveBetween(2000, 2100);
        RuleFor(x => x.RuleSetId).NotEmpty();
    }
}
