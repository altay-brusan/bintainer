using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.Reports.GetLowStock;

internal sealed class GetLowStockQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetLowStockQuery, IReadOnlyCollection<LowStockResponse>>
{
    public async Task<Result<IReadOnlyCollection<LowStockResponse>>> Handle(GetLowStockQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT
                p.id AS Id,
                p.part_number AS PartNumber,
                p.description AS Description,
                COALESCE(SUM(c.quantity), 0) AS TotalQuantity,
                p.low_stock_threshold AS LowStockThreshold
            FROM inventory.components p
            LEFT JOIN inventory.compartments c ON c.component_id = p.id
            WHERE p.low_stock_threshold > 0
            GROUP BY p.id, p.part_number, p.description, p.low_stock_threshold
            HAVING COALESCE(SUM(c.quantity), 0) <= p.low_stock_threshold
            ORDER BY p.part_number
            """;

        var rows = (await connection.QueryAsync<LowStockResponse>(sql)).ToList();

        return rows;
    }
}
