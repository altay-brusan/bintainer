using FluentValidation;

namespace Bintainer.Modules.Inventory.Application.Compartments.AssignComponent;

internal sealed class AssignComponentToCompartmentCommandValidator : AbstractValidator<AssignComponentToCompartmentCommand>
{
    public AssignComponentToCompartmentCommandValidator()
    {
        RuleFor(x => x.CompartmentId).NotEmpty();
        RuleFor(x => x.ComponentId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
