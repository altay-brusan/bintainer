using System.Data.Common;
using System.Text.Json;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.Domain.Components;
using Dapper;

namespace Bintainer.Modules.Catalog.Application.Components.GetComponent;

internal sealed class GetComponentQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetComponentQuery, ComponentResponse>
{
    public async Task<Result<ComponentResponse>> Handle(GetComponentQuery request, CancellationToken cancellationToken)
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
            FROM inventory.components p
            LEFT JOIN inventory.categories c ON c.id = p.category_id
            LEFT JOIN inventory.footprints f ON f.id = p.footprint_id
            WHERE p.id = @ComponentId
            """;

        var row = await connection.QueryFirstOrDefaultAsync<ComponentRow>(sql, new { request.ComponentId });

        if (row is null)
        {
            return Result.Failure<ComponentResponse>(ComponentErrors.NotFound(request.ComponentId));
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

        var locations = (await connection.QueryAsync<ComponentLocationResponse>(locationsSql, new { request.ComponentId })).ToList();

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
