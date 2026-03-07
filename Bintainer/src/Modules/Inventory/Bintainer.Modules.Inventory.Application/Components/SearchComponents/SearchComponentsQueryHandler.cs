using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.Components.SearchComponents;

internal sealed class SearchComponentsQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<SearchComponentsQuery, SearchComponentsPagedResponse>
{
    public async Task<Result<SearchComponentsPagedResponse>> Handle(SearchComponentsQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(request.Q))
        {
            conditions.Add("(p.part_number ILIKE @Q OR p.manufacturer_part_number ILIKE @Q OR p.description ILIKE @Q)");
            parameters.Add("Q", $"%{request.Q}%");
        }

        if (request.CategoryId.HasValue)
        {
            conditions.Add("p.category_id = @CategoryId");
            parameters.Add("CategoryId", request.CategoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(request.Provider))
        {
            conditions.Add("p.provider ILIKE @Provider");
            parameters.Add("Provider", $"%{request.Provider}%");
        }

        if (!string.IsNullOrWhiteSpace(request.Tag))
        {
            conditions.Add("p.tags ILIKE @Tag");
            parameters.Add("Tag", $"%{request.Tag}%");
        }

        if (request.FootprintId.HasValue)
        {
            conditions.Add("p.footprint_id = @FootprintId");
            parameters.Add("FootprintId", request.FootprintId.Value);
        }

        var whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";

        var countSql = $"SELECT COUNT(*) FROM inventory.components p {whereClause}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize > 100 ? 100 : request.PageSize;
        var offset = (page - 1) * pageSize;

        parameters.Add("Limit", pageSize);
        parameters.Add("Offset", offset);

        var sql = $"""
            SELECT
                p.id AS Id, p.part_number AS PartNumber,
                p.manufacturer_part_number AS ManufacturerPartNumber,
                p.description AS Description, p.image_url AS ImageUrl,
                p.category_id AS CategoryId, c.name AS CategoryName,
                p.footprint_id AS FootprintId, f.name AS FootprintName,
                p.tags AS Tags,
                p.unit_price AS UnitPrice,
                p.manufacturer AS Manufacturer,
                COALESCE((SELECT SUM(comp.quantity) FROM inventory.compartments comp WHERE comp.component_id = p.id), 0) AS TotalQuantity
            FROM inventory.components p
            LEFT JOIN inventory.categories c ON c.id = p.category_id
            LEFT JOIN inventory.footprints f ON f.id = p.footprint_id
            {whereClause}
            ORDER BY p.part_number
            LIMIT @Limit OFFSET @Offset
            """;

        var items = (await connection.QueryAsync<SearchComponentItemResponse>(sql, parameters)).ToList();

        return new SearchComponentsPagedResponse(totalCount, page, pageSize, items);
    }
}
