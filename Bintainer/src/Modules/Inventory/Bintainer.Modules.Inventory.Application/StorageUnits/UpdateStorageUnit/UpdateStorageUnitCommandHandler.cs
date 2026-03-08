using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Application.StorageUnits.UpdateStorageUnit;

internal sealed class UpdateStorageUnitCommandHandler(
    IStorageUnitRepository storageUnitRepository,
    IUnitOfWork unitOfWork,
    IActivityLogger activityLogger,
    ICurrentUserService currentUserService) : ICommandHandler<UpdateStorageUnitCommand>
{
    public async Task<Result> Handle(UpdateStorageUnitCommand request, CancellationToken cancellationToken)
    {
        StorageUnit? storageUnit = await storageUnitRepository.GetByIdAsync(
            request.StorageUnitId, cancellationToken);

        if (storageUnit is null)
        {
            return Result.Failure(StorageUnitErrors.NotFound(request.StorageUnitId));
        }

        storageUnit.Update(request.Name);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await activityLogger.LogAsync(
            currentUserService.UserId,
            "StorageUnitUpdated",
            "StorageUnit",
            storageUnit.Id,
            request.Name,
            ct: cancellationToken);

        return Result.Success();
    }
}
