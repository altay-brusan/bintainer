using System.Data.Common;
using System.Text.Json;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Domain.Parts;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.Parts.GetPart;

internal sealed class GetPartQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetPartQuery, PartResponse>
{
    public async Task<Result<PartResponse>> Handle(GetPartQuery request, CancellationToken cancellationToken)
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
                p.tags AS Tags
            FROM inventory.parts p
            LEFT JOIN inventory.categories c ON c.id = p.category_id
            LEFT JOIN inventory.footprints f ON f.id = p.footprint_id
            WHERE p.id = @PartId
            """;

        var row = await connection.QueryFirstOrDefaultAsync<PartRow>(sql, new { request.PartId });

        if (row is null)
        {
            return Result.Failure<PartResponse>(PartErrors.NotFound(request.PartId));
        }

        var attributes = string.IsNullOrEmpty(row.Attributes)
            ? new Dictionary<string, string>()
            : JsonSerializer.Deserialize<Dictionary<string, string>>(row.Attributes) ?? [];

        return new PartResponse(
            row.Id, row.PartNumber, row.ManufacturerPartNumber,
            row.Description, row.DetailedDescription,
            row.ImageUrl, row.Url, row.Provider, row.ProviderPartNumber,
            row.CategoryId, row.CategoryName,
            row.FootprintId, row.FootprintName,
            attributes,
            row.Tags);
    }

    private sealed record PartRow(
        Guid Id, string PartNumber, string ManufacturerPartNumber,
        string Description, string? DetailedDescription,
        string? ImageUrl, string? Url, string? Provider, string? ProviderPartNumber,
        Guid? CategoryId, string? CategoryName,
        Guid? FootprintId, string? FootprintName,
        string? Attributes,
        string? Tags);
}
