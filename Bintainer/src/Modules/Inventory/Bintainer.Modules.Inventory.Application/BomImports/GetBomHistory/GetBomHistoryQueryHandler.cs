using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.BomImports.GetBomHistory;

internal sealed class GetBomHistoryQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetBomHistoryQuery, BomHistoryPagedResponse>
{
    public async Task<Result<BomHistoryPagedResponse>> Handle(GetBomHistoryQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize > 100 ? 100 : request.PageSize;
        var offset = (page - 1) * pageSize;

        var totalCount = await connection.ExecuteScalarAsync<int>("SELECT COUNT(*) FROM inventory.bom_imports");

        const string sql =
            """
            SELECT
                id AS Id, file_name AS FileName, total_lines AS TotalLines,
                matched_count AS MatchedCount, new_count AS NewCount,
                total_value AS TotalValue, user_id AS UserId, date AS Date
            FROM inventory.bom_imports
            ORDER BY date DESC
            LIMIT @Limit OFFSET @Offset
            """;

        var items = (await connection.QueryAsync<BomHistoryItemResponse>(sql, new { Limit = pageSize, Offset = offset })).ToList();

        return new BomHistoryPagedResponse(totalCount, page, pageSize, items);
    }
}
