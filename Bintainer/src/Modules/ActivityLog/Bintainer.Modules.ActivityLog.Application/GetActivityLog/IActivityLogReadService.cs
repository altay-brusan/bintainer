namespace Bintainer.Modules.ActivityLog.Application.GetActivityLog;

public interface IActivityLogReadService
{
    Task<ActivityLogPagedResponse> GetPagedAsync(
        string? action,
        string? entityType,
        Guid? entityId,
        int page,
        int pageSize,
        CancellationToken ct);
}
