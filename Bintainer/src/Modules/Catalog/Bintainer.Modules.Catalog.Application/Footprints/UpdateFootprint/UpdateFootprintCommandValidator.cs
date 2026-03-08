using FluentValidation;

namespace Bintainer.Modules.Catalog.Application.Footprints.UpdateFootprint;

internal sealed class UpdateFootprintCommandValidator : AbstractValidator<UpdateFootprintCommand>
{
    public UpdateFootprintCommandValidator()
    {
        RuleFor(x => x.FootprintId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
