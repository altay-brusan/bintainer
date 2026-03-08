using Bintainer.Modules.Inventory.Application.Inventories.GetInventories;
using Bintainer.Modules.Inventory.Application.Inventories.GetInventory;

namespace Bintainer.Modules.Inventory.Application.Inventories;

public interface IInventoryReadService
{
    Task<IReadOnlyCollection<InventorySummaryResponse>> GetByUserIdAsync(Guid userId, CancellationToken ct);
    Task<InventoryResponse?> GetByIdAsync(Guid inventoryId, CancellationToken ct);
}
