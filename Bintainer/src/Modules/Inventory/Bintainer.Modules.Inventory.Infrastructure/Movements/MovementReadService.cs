using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Modules.Inventory.Application.Movements;
using Bintainer.Modules.Inventory.Application.Movements.GetMovements;
using Dapper;

namespace Bintainer.Modules.Inventory.Infrastructure.Movements;

internal sealed class MovementReadService(IDbConnectionFactory dbConnectionFactory) : IMovementReadService
{
    public async Task<MovementsPagedResponse> GetPagedAsync(string? action, Guid? componentId, string? q, int page, int pageSize, CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(action))
        {
            conditions.Add("m.action = @Action");
            parameters.Add("Action", action);
        }

        if (componentId.HasValue)
        {
            conditions.Add("m.component_id = @ComponentId");
            parameters.Add("ComponentId", componentId.Value);
        }

        if (!string.IsNullOrWhiteSpace(q))
        {
            conditions.Add("(comp.part_number ILIKE @Q OR comp.description ILIKE @Q)");
            parameters.Add("Q", $"%{q}%");
        }

        var whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";

        var countSql = $"""
            SELECT COUNT(*)
            FROM inventory.movements m
            LEFT JOIN catalog.components comp ON comp.id = m.component_id
            {whereClause}
            """;
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : pageSize > 100 ? 100 : pageSize;
        var offset = (page - 1) * pageSize;

        parameters.Add("Limit", pageSize);
        parameters.Add("Offset", offset);

        var sql = $"""
            SELECT
                m.id AS Id, m.date AS Date,
                m.component_id AS ComponentId, comp.part_number AS ComponentPartNumber,
                m.action AS Action, m.quantity AS Quantity,
                m.compartment_id AS CompartmentId, c.label AS CompartmentLabel,
                m.source_compartment_id AS SourceCompartmentId, sc.label AS SourceCompartmentLabel,
                su.name AS StorageUnitName,
                m.user_id AS UserId,
                u.first_name || ' ' || u.last_name AS UserName,
                m.notes AS Notes
            FROM inventory.movements m
            LEFT JOIN catalog.components comp ON comp.id = m.component_id
            LEFT JOIN inventory.compartments c ON c.id = m.compartment_id
            LEFT JOIN inventory.bins b ON b.id = c.bin_id
            LEFT JOIN inventory.storage_units su ON su.id = b.storage_unit_id
            LEFT JOIN inventory.compartments sc ON sc.id = m.source_compartment_id
            LEFT JOIN users.domain_users u ON u.id = m.user_id
            {whereClause}
            ORDER BY m.date DESC
            LIMIT @Limit OFFSET @Offset
            """;

        var items = (await connection.QueryAsync<MovementItemResponse>(sql, parameters)).ToList();

        return new MovementsPagedResponse(totalCount, page, pageSize, items);
    }
}
