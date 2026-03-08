using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Application.StorageUnits.GetStorageUnits;

internal sealed class GetStorageUnitsQueryHandler(
    IStorageUnitReadService storageUnitReadService) : IQueryHandler<GetStorageUnitsQuery, IReadOnlyCollection<StorageUnitSummaryResponse>>
{
    public async Task<Result<IReadOnlyCollection<StorageUnitSummaryResponse>>> Handle(
        GetStorageUnitsQuery request, CancellationToken cancellationToken)
    {
        var storageUnits = await storageUnitReadService.GetByInventoryIdAsync(request.InventoryId, cancellationToken);
        return Result.Success(storageUnits);
    }
}
