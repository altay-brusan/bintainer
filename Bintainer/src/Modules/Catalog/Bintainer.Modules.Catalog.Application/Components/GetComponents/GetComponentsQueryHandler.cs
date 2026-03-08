using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Dapper;

namespace Bintainer.Modules.Catalog.Application.Components.GetComponents;

internal sealed class GetComponentsQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetComponentsQuery, IReadOnlyCollection<ComponentSummaryResponse>>
{
    public async Task<Result<IReadOnlyCollection<ComponentSummaryResponse>>> Handle(GetComponentsQuery request, CancellationToken cancellationToken)
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
            FROM inventory.components p
            LEFT JOIN inventory.categories c ON c.id = p.category_id
            LEFT JOIN inventory.footprints f ON f.id = p.footprint_id
            """;

        if (request.CategoryId.HasValue)
        {
            sql += " WHERE p.category_id = @CategoryId";
        }

        sql += " ORDER BY p.part_number";

        var components = await connection.QueryAsync<ComponentSummaryResponse>(sql, new { request.CategoryId });

        return components.ToList();
    }
}
