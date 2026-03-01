using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.StorageUnits.GetStorageUnits;

internal sealed class GetStorageUnitsQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetStorageUnitsQuery, IReadOnlyCollection<StorageUnitSummaryResponse>>
{
    public async Task<Result<IReadOnlyCollection<StorageUnitSummaryResponse>>> Handle(
        GetStorageUnitsQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT
                id AS Id,
                name AS Name,
                columns AS Columns,
                rows AS Rows,
                compartment_count AS CompartmentCount,
                inventory_id AS InventoryId
            FROM inventory.storage_units
            WHERE inventory_id = @InventoryId
            ORDER BY name
            """;

        var storageUnits = await connection.QueryAsync<StorageUnitSummaryResponse>(
            sql, new { request.InventoryId });

        return storageUnits.ToList().AsReadOnly();
    }
}
