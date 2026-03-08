using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Application.Inventories.GetInventories;

internal sealed class GetInventoriesQueryHandler(
    IInventoryReadService inventoryReadService) : IQueryHandler<GetInventoriesQuery, IReadOnlyCollection<InventorySummaryResponse>>
{
    public async Task<Result<IReadOnlyCollection<InventorySummaryResponse>>> Handle(
        GetInventoriesQuery request, CancellationToken cancellationToken)
    {
        var inventories = await inventoryReadService.GetByUserIdAsync(request.UserId, cancellationToken);
        return Result.Success(inventories);
    }
}
