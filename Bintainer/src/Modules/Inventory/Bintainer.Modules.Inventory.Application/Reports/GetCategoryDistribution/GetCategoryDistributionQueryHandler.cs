using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Application.Reports.GetCategoryDistribution;

internal sealed class GetCategoryDistributionQueryHandler(
    IReportReadService reportReadService) : IQueryHandler<GetCategoryDistributionQuery, IReadOnlyCollection<CategoryDistributionResponse>>
{
    public async Task<Result<IReadOnlyCollection<CategoryDistributionResponse>>> Handle(GetCategoryDistributionQuery request, CancellationToken cancellationToken)
    {
        var rows = await reportReadService.GetCategoryDistributionAsync(cancellationToken);
        return Result.Success(rows);
    }
}
