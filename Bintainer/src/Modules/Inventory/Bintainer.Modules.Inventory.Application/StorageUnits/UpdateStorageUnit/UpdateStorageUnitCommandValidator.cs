using FluentValidation;

namespace Bintainer.Modules.Inventory.Application.StorageUnits.UpdateStorageUnit;

internal sealed class UpdateStorageUnitCommandValidator : AbstractValidator<UpdateStorageUnitCommand>
{
    public UpdateStorageUnitCommandValidator()
    {
        RuleFor(x => x.StorageUnitId).NotEmpty();
        RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
    }
}
