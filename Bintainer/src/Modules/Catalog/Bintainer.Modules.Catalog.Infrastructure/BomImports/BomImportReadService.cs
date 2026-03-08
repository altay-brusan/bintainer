using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Modules.Catalog.Application.BomImports;
using Bintainer.Modules.Catalog.Application.BomImports.GetBomHistory;
using Dapper;

namespace Bintainer.Modules.Catalog.Infrastructure.BomImports;

internal sealed class BomImportReadService(IDbConnectionFactory dbConnectionFactory) : IBomImportReadService
{
    public async Task<BomHistoryPagedResponse> GetPagedAsync(int page, int pageSize, CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : pageSize > 100 ? 100 : pageSize;
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
