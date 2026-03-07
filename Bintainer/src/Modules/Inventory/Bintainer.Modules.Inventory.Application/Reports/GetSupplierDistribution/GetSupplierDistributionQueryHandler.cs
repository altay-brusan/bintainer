using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.Reports.GetSupplierDistribution;

internal sealed class GetSupplierDistributionQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetSupplierDistributionQuery, IReadOnlyCollection<SupplierDistributionResponse>>
{
    public async Task<Result<IReadOnlyCollection<SupplierDistributionResponse>>> Handle(GetSupplierDistributionQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT
                COALESCE(provider, 'Unknown') AS SupplierName,
                COUNT(*) AS ComponentCount
            FROM inventory.components
            WHERE provider IS NOT NULL AND provider != ''
            GROUP BY provider
            ORDER BY ComponentCount DESC
            """;

        var rows = (await connection.QueryAsync<SupplierDistributionResponse>(sql)).ToList();

        return rows;
    }
}
