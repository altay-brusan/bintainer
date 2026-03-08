using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Application.StorageUnits.GetStorageUnit;

internal sealed class GetStorageUnitQueryHandler(
    IStorageUnitReadService storageUnitReadService) : IQueryHandler<GetStorageUnitQuery, StorageUnitResponse>
{
    public async Task<Result<StorageUnitResponse>> Handle(GetStorageUnitQuery request, CancellationToken cancellationToken)
    {
        var result = await storageUnitReadService.GetByIdAsync(request.StorageUnitId, cancellationToken);

        if (result is null)
        {
            return Result.Failure<StorageUnitResponse>(StorageUnitErrors.NotFound(request.StorageUnitId));
        }

        return result;
    }
}
