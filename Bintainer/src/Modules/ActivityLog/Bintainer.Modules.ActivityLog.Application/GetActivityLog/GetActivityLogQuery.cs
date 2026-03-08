using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.ActivityLog.Application.GetActivityLog;

public sealed record GetActivityLogQuery(
    string? Action,
    string? EntityType,
    Guid? EntityId,
    int Page,
    int PageSize) : IQuery<ActivityLogPagedResponse>;

public sealed record ActivityLogPagedResponse(
    int TotalCount,
    int Page,
    int PageSize,
    List<ActivityLogItemResponse> Items);

public sealed record ActivityLogItemResponse(
    Guid Id,
    Guid UserId,
    string? UserName,
    string Action,
    string EntityType,
    Guid EntityId,
    string? EntityName,
    string? Message,
    string? Details,
    DateTime Timestamp);
