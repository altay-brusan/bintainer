using Bintainer.Modules.Inventory.Application.StorageUnits.GetStorageUnit;
using Bintainer.Modules.Inventory.Application.StorageUnits.GetStorageUnits;

namespace Bintainer.Modules.Inventory.Application.StorageUnits;

public interface IStorageUnitReadService
{
    Task<IReadOnlyCollection<StorageUnitSummaryResponse>> GetByInventoryIdAsync(Guid inventoryId, CancellationToken ct);
    Task<StorageUnitResponse?> GetByIdAsync(Guid storageUnitId, CancellationToken ct);
}
