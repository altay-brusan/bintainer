using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Application.StorageUnits.DeleteStorageUnit;

internal sealed class DeleteStorageUnitCommandHandler(
    IStorageUnitRepository storageUnitRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteStorageUnitCommand>
{
    public async Task<Result> Handle(DeleteStorageUnitCommand request, CancellationToken cancellationToken)
    {
        StorageUnit? storageUnit = await storageUnitRepository.GetByIdAsync(
            request.StorageUnitId, cancellationToken);

        if (storageUnit is null)
        {
            return Result.Failure(StorageUnitErrors.NotFound(request.StorageUnitId));
        }

        storageUnit.Raise(new StorageUnitDeletedDomainEvent(storageUnit.Id, storageUnit.Name));

        storageUnitRepository.Remove(storageUnit);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
