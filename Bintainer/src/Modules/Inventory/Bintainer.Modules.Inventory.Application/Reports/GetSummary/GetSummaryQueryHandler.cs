using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Application.Reports.GetSummary;

internal sealed class GetSummaryQueryHandler(
    IReportReadService reportReadService) : IQueryHandler<GetSummaryQuery, SummaryResponse>
{
    public async Task<Result<SummaryResponse>> Handle(GetSummaryQuery request, CancellationToken cancellationToken)
    {
        var result = await reportReadService.GetSummaryAsync(cancellationToken);
        return result;
    }
}
