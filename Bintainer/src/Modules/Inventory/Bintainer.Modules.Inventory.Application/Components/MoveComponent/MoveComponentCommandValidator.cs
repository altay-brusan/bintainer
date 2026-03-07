using FluentValidation;

namespace Bintainer.Modules.Inventory.Application.Components.MoveComponent;

internal sealed class MoveComponentCommandValidator : AbstractValidator<MoveComponentCommand>
{
    public MoveComponentCommandValidator()
    {
        RuleFor(x => x.ComponentId).NotEmpty();
        RuleFor(x => x.SourceCompartmentId).NotEmpty();
        RuleFor(x => x.DestinationCompartmentId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
