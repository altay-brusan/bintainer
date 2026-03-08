using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.ActivityLog.Application.GetActivityLog;

internal sealed class GetActivityLogQueryHandler(
    IActivityLogReadService activityLogReadService) : IQueryHandler<GetActivityLogQuery, ActivityLogPagedResponse>
{
    public async Task<Result<ActivityLogPagedResponse>> Handle(GetActivityLogQuery request, CancellationToken cancellationToken)
    {
        var result = await activityLogReadService.GetPagedAsync(
            request.Action, request.EntityType, request.EntityId,
            request.Page, request.PageSize, cancellationToken);
        return result;
    }
}
