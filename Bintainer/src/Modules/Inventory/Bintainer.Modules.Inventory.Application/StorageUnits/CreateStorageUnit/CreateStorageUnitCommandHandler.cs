using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Inventories;
using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Application.StorageUnits.CreateStorageUnit;

internal sealed class CreateStorageUnitCommandHandler(
    IInventoryRepository inventoryRepository,
    IStorageUnitRepository storageUnitRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateStorageUnitCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateStorageUnitCommand request, CancellationToken cancellationToken)
    {
        Domain.Inventories.Inventory? inventory = await inventoryRepository.GetByIdAsync(
            request.InventoryId, cancellationToken);

        if (inventory is null)
        {
            return Result.Failure<Guid>(InventoryErrors.NotFound(request.InventoryId));
        }

        var storageUnit = StorageUnit.Create(
            request.Name,
            request.Columns,
            request.Rows,
            request.CompartmentCount,
            request.InventoryId);

        storageUnitRepository.Insert(storageUnit);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return storageUnit.Id;
    }
}
