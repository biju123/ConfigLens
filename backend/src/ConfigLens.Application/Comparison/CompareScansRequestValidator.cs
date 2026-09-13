using ConfigLens.Domain.Scan;
using FluentValidation;

namespace ConfigLens.Application.Comparison;

public sealed class CompareScansRequestValidator : AbstractValidator<CompareScansRequest>
{
    public CompareScansRequestValidator()
    {
        RuleFor(x => x.CurrentScanId).NotEmpty().Must(BeAValidScanId).WithMessage("'{PropertyValue}' is not a valid scan id.");
        RuleFor(x => x.BaselineScanId).NotEmpty().Must(BeAValidScanId).WithMessage("'{PropertyValue}' is not a valid scan id.");
    }

    private static bool BeAValidScanId(string value) => ScanId.TryParse(value, out _);
}
