using FluentValidation;

namespace Bintainer.Modules.Inventory.Application.Components.AdjustComponentQuantity;

internal sealed class AdjustComponentQuantityCommandValidator : AbstractValidator<AdjustComponentQuantityCommand>
{
    public AdjustComponentQuantityCommandValidator()
    {
        RuleFor(x => x.ComponentId).NotEmpty();
        RuleFor(x => x.CompartmentId).NotEmpty();
        RuleFor(x => x.Action).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0);
    }
}
