using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.Reports.GetSummary;

internal sealed class GetSummaryQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetSummaryQuery, SummaryResponse>
{
    public async Task<Result<SummaryResponse>> Handle(GetSummaryQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT
                (SELECT COUNT(*) FROM inventory.components) AS TotalComponents,
                (SELECT COUNT(*) FROM inventory.categories) AS TotalCategories,
                (SELECT COUNT(*) FROM inventory.storage_units) AS TotalStorageUnits,
                (SELECT COUNT(*) FROM inventory.compartments WHERE component_id IS NOT NULL) AS OccupiedCompartments,
                (SELECT COALESCE(SUM(quantity), 0) FROM inventory.compartments WHERE component_id IS NOT NULL) AS TotalQuantity,
                (SELECT COALESCE(SUM(c.quantity * COALESCE(p.unit_price, 0)), 0)
                 FROM inventory.compartments c
                 JOIN inventory.components p ON p.id = c.component_id
                 WHERE c.component_id IS NOT NULL) AS TotalValue,
                (SELECT COUNT(*) FROM inventory.movements WHERE date >= NOW() - INTERVAL '30 days') AS RecentMovements
            """;

        var result = await connection.QuerySingleAsync<SummaryResponse>(sql);

        return result;
    }
}
