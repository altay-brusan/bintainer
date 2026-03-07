using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.Reports.GetTopComponents;

internal sealed class GetTopComponentsQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetTopComponentsQuery, IReadOnlyCollection<TopComponentResponse>>
{
    public async Task<Result<IReadOnlyCollection<TopComponentResponse>>> Handle(GetTopComponentsQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        var limit = request.Limit < 1 ? 10 : request.Limit > 50 ? 50 : request.Limit;
        var orderBy = request.SortBy?.ToLowerInvariant() == "value" ? "TotalValue" : "TotalQuantity";

        var sql = $"""
            SELECT
                p.id AS Id,
                p.part_number AS PartNumber,
                p.description AS Description,
                COALESCE(SUM(c.quantity), 0) AS TotalQuantity,
                COALESCE(SUM(c.quantity * COALESCE(p.unit_price, 0)), 0) AS TotalValue
            FROM inventory.components p
            LEFT JOIN inventory.compartments c ON c.component_id = p.id
            GROUP BY p.id, p.part_number, p.description
            ORDER BY {orderBy} DESC
            LIMIT @Limit
            """;

        var rows = (await connection.QueryAsync<TopComponentResponse>(sql, new { Limit = limit })).ToList();

        return rows;
    }
}
