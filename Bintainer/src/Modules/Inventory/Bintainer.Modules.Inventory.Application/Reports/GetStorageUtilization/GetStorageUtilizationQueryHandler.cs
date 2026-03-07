using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.Reports.GetStorageUtilization;

internal sealed class GetStorageUtilizationQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetStorageUtilizationQuery, IReadOnlyCollection<StorageUtilizationResponse>>
{
    public async Task<Result<IReadOnlyCollection<StorageUtilizationResponse>>> Handle(GetStorageUtilizationQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT
                su.id AS StorageUnitId,
                su.name AS StorageUnitName,
                COUNT(c.id) AS TotalCompartments,
                COUNT(c.component_id) AS OccupiedCompartments
            FROM inventory.storage_units su
            JOIN inventory.bins b ON b.storage_unit_id = su.id
            JOIN inventory.compartments c ON c.bin_id = b.id
            GROUP BY su.id, su.name
            ORDER BY su.name
            """;

        var rows = (await connection.QueryAsync<StorageUtilizationResponse>(sql)).ToList();

        return rows;
    }
}
