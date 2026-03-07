using FluentValidation;

namespace Bintainer.Modules.Inventory.Application.Footprints.CreateFootprint;

internal sealed class CreateFootprintCommandValidator : AbstractValidator<CreateFootprintCommand>
{
    public CreateFootprintCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
    }
}
