using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.Reports.GetCategoryDistribution;

internal sealed class GetCategoryDistributionQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetCategoryDistributionQuery, IReadOnlyCollection<CategoryDistributionResponse>>
{
    public async Task<Result<IReadOnlyCollection<CategoryDistributionResponse>>> Handle(GetCategoryDistributionQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT
                COALESCE(c.name, 'Uncategorized') AS CategoryName,
                COUNT(p.id) AS ComponentCount,
                COALESCE(SUM(COALESCE(p.unit_price, 0)), 0) AS TotalValue
            FROM inventory.components p
            LEFT JOIN inventory.categories c ON c.id = p.category_id
            GROUP BY c.name
            ORDER BY ComponentCount DESC
            """;

        var rows = (await connection.QueryAsync<CategoryDistributionResponse>(sql)).ToList();

        return rows;
    }
}
