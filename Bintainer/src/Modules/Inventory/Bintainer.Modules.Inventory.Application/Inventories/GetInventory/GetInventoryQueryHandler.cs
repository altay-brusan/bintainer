using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Domain.Inventories;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.Inventories.GetInventory;

internal sealed class GetInventoryQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetInventoryQuery, InventoryResponse>
{
    public async Task<Result<InventoryResponse>> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT id AS Id, name AS Name, user_id AS UserId
            FROM inventory.inventories
            WHERE id = @InventoryId
            """;

        var inventory = await connection.QueryFirstOrDefaultAsync<InventoryResponse>(
            sql, new { request.InventoryId });

        if (inventory is null)
        {
            return Result.Failure<InventoryResponse>(InventoryErrors.NotFound(request.InventoryId));
        }

        return inventory;
    }
}
