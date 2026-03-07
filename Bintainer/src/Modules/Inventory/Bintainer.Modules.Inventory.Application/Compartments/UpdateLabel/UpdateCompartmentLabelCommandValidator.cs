using FluentValidation;

namespace Bintainer.Modules.Inventory.Application.Compartments.UpdateLabel;

internal sealed class UpdateCompartmentLabelCommandValidator : AbstractValidator<UpdateCompartmentLabelCommand>
{
    public UpdateCompartmentLabelCommandValidator()
    {
        RuleFor(x => x.CompartmentId).NotEmpty();
        RuleFor(x => x.Label).NotEmpty().MaximumLength(100);
    }
}
