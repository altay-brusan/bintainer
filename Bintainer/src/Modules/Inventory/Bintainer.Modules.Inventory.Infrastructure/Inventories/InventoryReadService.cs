using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Modules.Inventory.Application.Inventories;
using Bintainer.Modules.Inventory.Application.Inventories.GetInventories;
using Bintainer.Modules.Inventory.Application.Inventories.GetInventory;
using Dapper;

namespace Bintainer.Modules.Inventory.Infrastructure.Inventories;

internal sealed class InventoryReadService(IDbConnectionFactory dbConnectionFactory) : IInventoryReadService
{
    public async Task<IReadOnlyCollection<InventorySummaryResponse>> GetByUserIdAsync(Guid userId, CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT id AS Id, name AS Name, user_id AS UserId
            FROM inventory.inventories
            WHERE user_id = @UserId
            ORDER BY name
            """;

        var inventories = await connection.QueryAsync<InventorySummaryResponse>(sql, new { UserId = userId });
        return inventories.ToList().AsReadOnly();
    }

    public async Task<InventoryResponse?> GetByIdAsync(Guid inventoryId, CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT id AS Id, name AS Name, user_id AS UserId
            FROM inventory.inventories
            WHERE id = @InventoryId
            """;

        return await connection.QueryFirstOrDefaultAsync<InventoryResponse>(sql, new { InventoryId = inventoryId });
    }
}
