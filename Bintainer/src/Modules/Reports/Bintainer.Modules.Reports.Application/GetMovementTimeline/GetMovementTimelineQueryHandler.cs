using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.Reports.Application.GetMovementTimeline;

internal sealed class GetMovementTimelineQueryHandler(
    IReportReadService reportReadService) : IQueryHandler<GetMovementTimelineQuery, IReadOnlyCollection<MovementTimelineResponse>>
{
    public async Task<Result<IReadOnlyCollection<MovementTimelineResponse>>> Handle(GetMovementTimelineQuery request, CancellationToken cancellationToken)
    {
        var rows = await reportReadService.GetMovementTimelineAsync(request.Days, cancellationToken);
        return Result.Success(rows);
    }
}
