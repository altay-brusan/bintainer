using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Domain.Inventories;

namespace Bintainer.Modules.Inventory.Application.Inventories.GetInventory;

internal sealed class GetInventoryQueryHandler(
    IInventoryReadService inventoryReadService) : IQueryHandler<GetInventoryQuery, InventoryResponse>
{
    public async Task<Result<InventoryResponse>> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
    {
        var inventory = await inventoryReadService.GetByIdAsync(request.InventoryId, cancellationToken);

        if (inventory is null)
        {
            return Result.Failure<InventoryResponse>(InventoryErrors.NotFound(request.InventoryId));
        }

        return inventory;
    }
}
