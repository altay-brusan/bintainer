using Bintainer.Common.Domain;

namespace Bintainer.Modules.ActivityLog.Domain;

public sealed class ActivityEntry : Entity
{
    public Guid UserId { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string? EntityName { get; private set; }
    public string? Message { get; private set; }
    public string? Details { get; private set; }
    public DateTime Timestamp { get; private set; }

    private ActivityEntry() { }

    public static ActivityEntry Create(
        Guid userId,
        string action,
        string entityType,
        Guid entityId,
        string? entityName,
        string? message,
        string? details)
    {
        return new ActivityEntry
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            EntityName = entityName,
            Message = message,
            Details = details,
            Timestamp = DateTime.UtcNow
        };
    }
}
