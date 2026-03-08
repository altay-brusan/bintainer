namespace Bintainer.Common.Application.ActivityLog;

public interface IActivityLogger
{
    Task LogAsync(
        Guid userId,
        string action,
        string entityType,
        Guid entityId,
        string? entityName = null,
        object? details = null,
        CancellationToken ct = default);
}
