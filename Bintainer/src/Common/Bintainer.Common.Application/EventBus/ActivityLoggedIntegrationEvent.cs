namespace Bintainer.Common.Application.EventBus;

public sealed class ActivityLoggedIntegrationEvent(
    Guid id,
    DateTime occurredOnUtc,
    Guid userId,
    string action,
    string entityType,
    Guid entityId,
    string? entityName,
    string? message,
    string? details) : IntegrationEvent(id, occurredOnUtc)
{
    public Guid UserId { get; init; } = userId;
    public string Action { get; init; } = action;
    public string EntityType { get; init; } = entityType;
    public Guid EntityId { get; init; } = entityId;
    public string? EntityName { get; init; } = entityName;
    public string? Message { get; init; } = message;
    public string? Details { get; init; } = details;
}
