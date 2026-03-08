using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.Reports.Application.GetLowStock;

internal sealed class GetLowStockQueryHandler(
    IReportReadService reportReadService) : IQueryHandler<GetLowStockQuery, IReadOnlyCollection<LowStockResponse>>
{
    public async Task<Result<IReadOnlyCollection<LowStockResponse>>> Handle(GetLowStockQuery request, CancellationToken cancellationToken)
    {
        var rows = await reportReadService.GetLowStockAsync(cancellationToken);
        return Result.Success(rows);
    }
}
