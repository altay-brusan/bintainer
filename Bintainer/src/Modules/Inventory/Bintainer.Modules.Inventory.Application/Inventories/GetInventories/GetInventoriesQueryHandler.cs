using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.Inventories.GetInventories;

internal sealed class GetInventoriesQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetInventoriesQuery, IReadOnlyCollection<InventorySummaryResponse>>
{
    public async Task<Result<IReadOnlyCollection<InventorySummaryResponse>>> Handle(
        GetInventoriesQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT id AS Id, name AS Name, user_id AS UserId
            FROM inventory.inventories
            WHERE user_id = @UserId
            ORDER BY name
            """;

        var inventories = await connection.QueryAsync<InventorySummaryResponse>(
            sql, new { request.UserId });

        return inventories.ToList().AsReadOnly();
    }
}
