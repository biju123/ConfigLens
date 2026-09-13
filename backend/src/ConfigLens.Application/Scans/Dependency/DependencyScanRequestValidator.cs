using FluentValidation;

namespace ConfigLens.Application.Scans.Dependency;

public sealed class DependencyScanRequestValidator : AbstractValidator<DependencyScanRequest>
{
    public DependencyScanRequestValidator()
    {
        RuleFor(x => x.Application).NotEmpty();
        RuleFor(x => x.Environment).NotEmpty();
        RuleFor(x => x.Namespace).NotEmpty();
    }
}
