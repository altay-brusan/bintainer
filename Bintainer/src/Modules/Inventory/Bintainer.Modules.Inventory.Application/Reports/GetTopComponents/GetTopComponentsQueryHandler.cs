using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Application.Reports.GetTopComponents;

internal sealed class GetTopComponentsQueryHandler(
    IReportReadService reportReadService) : IQueryHandler<GetTopComponentsQuery, IReadOnlyCollection<TopComponentResponse>>
{
    public async Task<Result<IReadOnlyCollection<TopComponentResponse>>> Handle(GetTopComponentsQuery request, CancellationToken cancellationToken)
    {
        var rows = await reportReadService.GetTopComponentsAsync(request.SortBy, request.Limit, cancellationToken);
        return Result.Success(rows);
    }
}
