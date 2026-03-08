using System.Text.Json;
using Bintainer.Common.Application.ActivityLog;
using Bintainer.Modules.ActivityLog.Domain;
using Bintainer.Modules.ActivityLog.Infrastructure.Database;

namespace Bintainer.Modules.ActivityLog.Infrastructure;

internal sealed class ActivityLogger(ActivityLogDbContext dbContext) : IActivityLogger
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task LogAsync(
        Guid userId,
        string action,
        string entityType,
        Guid entityId,
        string? entityName = null,
        object? details = null,
        CancellationToken ct = default)
    {
        var detailsJson = details is not null
            ? JsonSerializer.Serialize(details, JsonOptions)
            : null;

        var entry = ActivityEntry.Create(userId, action, entityType, entityId, entityName, detailsJson);
        dbContext.Activities.Add(entry);
        await dbContext.SaveChangesAsync(ct);
    }
}
