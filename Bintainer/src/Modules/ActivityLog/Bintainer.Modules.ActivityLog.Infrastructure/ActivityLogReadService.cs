using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Modules.ActivityLog.Application.GetActivityLog;
using Dapper;

namespace Bintainer.Modules.ActivityLog.Infrastructure;

internal sealed class ActivityLogReadService(IDbConnectionFactory dbConnectionFactory) : IActivityLogReadService
{
    public async Task<ActivityLogPagedResponse> GetPagedAsync(
        string? action,
        string? entityType,
        Guid? entityId,
        int page,
        int pageSize,
        CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        var conditions = new List<string>();
        var parameters = new DynamicParameters();

        if (!string.IsNullOrWhiteSpace(action))
        {
            conditions.Add("a.action = @Action");
            parameters.Add("Action", action);
        }

        if (!string.IsNullOrWhiteSpace(entityType))
        {
            conditions.Add("a.entity_type = @EntityType");
            parameters.Add("EntityType", entityType);
        }

        if (entityId.HasValue)
        {
            conditions.Add("a.entity_id = @EntityId");
            parameters.Add("EntityId", entityId.Value);
        }

        var whereClause = conditions.Count > 0 ? "WHERE " + string.Join(" AND ", conditions) : "";

        var countSql = $"SELECT COUNT(*) FROM activity.activities a {whereClause}";
        var totalCount = await connection.ExecuteScalarAsync<int>(countSql, parameters);

        page = page < 1 ? 1 : page;
        pageSize = pageSize < 1 ? 20 : pageSize > 100 ? 100 : pageSize;
        var offset = (page - 1) * pageSize;

        parameters.Add("Limit", pageSize);
        parameters.Add("Offset", offset);

        var sql = $"""
            SELECT
                a.id AS Id,
                a.user_id AS UserId,
                u.first_name || ' ' || u.last_name AS UserName,
                a.action AS Action,
                a.entity_type AS EntityType,
                a.entity_id AS EntityId,
                a.entity_name AS EntityName,
                a.message AS Message,
                a.details AS Details,
                a.timestamp AS Timestamp
            FROM activity.activities a
            LEFT JOIN users.domain_users u ON u.id = a.user_id
            {whereClause}
            ORDER BY a.timestamp DESC
            LIMIT @Limit OFFSET @Offset
            """;

        var items = (await connection.QueryAsync<ActivityLogItemResponse>(sql, parameters)).ToList();

        return new ActivityLogPagedResponse(totalCount, page, pageSize, items);
    }
}
