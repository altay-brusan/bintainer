using FluentValidation;

namespace Bintainer.Modules.Inventory.Application.StorageUnits.CreateStorageUnit;

internal sealed class CreateStorageUnitCommandValidator : AbstractValidator<CreateStorageUnitCommand>
{
    public CreateStorageUnitCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        RuleFor(x => x.Columns).GreaterThan(0);
        RuleFor(x => x.Rows).GreaterThan(0);
        RuleFor(x => x.CompartmentCount).GreaterThan(0);
        RuleFor(x => x.InventoryId).NotEmpty();
    }
}
