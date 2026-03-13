using System.Data.Common;
using System.Text.Json;
using Bintainer.Common.Application.Data;
using Bintainer.Modules.Catalog.Application.Components;
using Bintainer.Modules.Catalog.Application.Components.GetComponent;
using Bintainer.Modules.Catalog.Application.Components.GetComponents;
using Bintainer.Modules.Catalog.Application.Components.SearchComponents;
using Dapper;

namespace Bintainer.Modules.Catalog.Infrastructure.Components;

internal sealed class ComponentReadService(IDbConnectionFactory dbConnectionFactory) : IComponentReadService
{
    public async Task<ComponentResponse?> GetByIdAsync(Guid id, CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT
                p.id AS Id, p.part_number AS PartNumber,
                p.manufacturer_part_number AS ManufacturerPartNumber,
                p.description AS Description, p.detailed_description AS DetailedDescription,
                p.image_url AS ImageUrl, p.url AS Url,
                p.provider AS Provider, p.provider_part_number AS ProviderPartNumber,
                p.category_id AS CategoryId, c.name AS CategoryName,
                p.footprint_id AS FootprintId, f.name AS FootprintName,
                p.attributes AS Attributes,
                p.tags AS Tags,
                p.unit_price AS UnitPrice,
                p.manufacturer AS Manufacturer,
                p.low_stock_threshold AS LowStockThreshold
            FROM catalog.components p
            LEFT JOIN catalog.categories c ON c.id = p.category_id
            LEFT JOIN catalog.footprints f ON f.id = p.footprint_id
            WHERE p.id = @ComponentId
            """;

        var row = await connection.QueryFirstOrDefaultAsync<ComponentRow>(sql, new { ComponentId = id });

        if (row is null)
        {
            return null;
        }

        var attributes = string.IsNullOrEmpty(row.Attributes)
            ? new Dictionary<string, string>()
            : JsonSerializer.Deserialize<Dictionary<string, string>>(row.Attributes) ?? [];

        const string locationsSql =
            """
            SELECT
                comp.id AS CompartmentId, comp.label AS Label, comp.quantity AS Quantity,
                comp.bin_id AS BinId, b.storage_unit_id AS StorageUnitId, su.name AS StorageUnitName
            FROM inventory.compartments comp
            INNER JOIN inventory.bins b ON b.id = comp.bin_id
            INNER JOIN inventory.storage_units su ON su.id = b.storage_unit_id
            WHERE comp.component_id = @ComponentId AND comp.quantity > 0
            """;

        var locations = (await connection.QueryAsync<ComponentLocationResponse>(locationsSql, new { ComponentId = id })).ToList();

        return new ComponentResponse(
            row.Id, row.PartNumber, row.ManufacturerPartNumber,
            row.Description, row.DetailedDescription,
            row.ImageUrl, row.Url, row.Provider, row.ProviderPartNumber,
            row.CategoryId, row.CategoryName,
            row.FootprintId, row.FootprintName,
            attributes,
            row.Tags,
            row.UnitPrice,
            row.Manufacturer,
            row.LowStockThreshold,
            locations);
    }

    public async Task<IReadOnlyCollection<ComponentSummaryResponse>> GetByCategoryIdAsync(Guid? categoryId, CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        var sql =
            """
            SELECT
                p.id AS Id, p.part_number AS PartNumber,
                p.manufacturer_part_number AS ManufacturerPartNumber,
                p.description AS Description, p.image_url AS ImageUrl,
                p.category_id AS CategoryId, c.name AS CategoryName,
                p.footprint_id AS FootprintId, f.name AS FootprintName,
                p.tags AS Tags,
                p.unit_price AS UnitPrice,
                p.manufacturer AS Manufacturer
            FROM catalog.components p
            LEFT JOIN catalog.categories c ON c.id = p.category_id
            LEFT JOIN catalog.footprints f ON f.id = p.footprint_id
            """;

        if (categoryId.HasValue)
        {
            sql += " WHERE p.category_id = @CategoryId";
        }

        sql += " ORDER BY p.part_number";

        var components = await connection.QueryAsync<ComponentSummaryResponse>(sql, new { CategoryId = categoryId });
        return components.ToList();
    }

    public async Task<SearchComponentsPagedResponse> SearchAsync(string? q, Guid? categoryId, string? provider, string? tag, Guid? footprintId, int page, int pageSize, CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(q))
        {
            conditions.Add("(p.part_number ILIKE @Q OR p.manufacturer_part_number ILIKE @Q OR p.description ILIKE @Q)");
            parameters.Add("Q", $"%{q}%");
        }

        if (categoryId.HasValue)
        {
            conditions.Add("p.category_id = @CategoryId");
            parameters.Add("CategoryId", categoryId.Value);
        }

        if (!string.IsNullOrWhiteSpace(provider))
        {
            conditions.Add("p.provider ILIKE @Provider");
            parameters.Add("Provider", $"%{provider}%");
        }

        if (!string.IsNullOrWhiteSpace(tag))
        {
            conditions.Add("p.tags ILIKE @Tag");
            parameters.Add("Tag", $"%{tag}%");
        }

        if (footprintId.HasValue)
        {
            conditions.Add("p.footprint_id = @FootprintId");
            parameters.Add("FootprintId", footprintId.Value);
        }

        var whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";

        var countSql = $"SELECT COUNT(*) FROM catalog.components p {whereClause}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : pageSize > 100 ? 100 : pageSize;
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
                COALESCE((SELECT SUM(comp.quantity) FROM inventory.compartments comp WHERE comp.component_id = p.id), 0)::int AS TotalQuantity
            FROM catalog.components p
            LEFT JOIN catalog.categories c ON c.id = p.category_id
            LEFT JOIN catalog.footprints f ON f.id = p.footprint_id
            {whereClause}
            ORDER BY p.part_number
            LIMIT @Limit OFFSET @Offset
            """;

        var items = (await connection.QueryAsync<SearchComponentItemResponse>(sql, parameters)).ToList();

        return new SearchComponentsPagedResponse(totalCount, page, pageSize, items);
    }

    public async Task<IReadOnlyCollection<string>> GetTagsAsync(CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql = "SELECT DISTINCT tags FROM catalog.components WHERE tags IS NOT NULL AND tags != ''";

        var rows = await connection.QueryAsync<string>(sql);

        var tags = rows
            .SelectMany(t => t.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return tags;
    }

    private sealed record ComponentRow(
        Guid Id, string PartNumber, string ManufacturerPartNumber,
        string Description, string? DetailedDescription,
        string? ImageUrl, string? Url, string? Provider, string? ProviderPartNumber,
        Guid? CategoryId, string? CategoryName,
        Guid? FootprintId, string? FootprintName,
        string? Attributes,
        string? Tags,
        decimal? UnitPrice,
        string? Manufacturer,
        int LowStockThreshold);
}
