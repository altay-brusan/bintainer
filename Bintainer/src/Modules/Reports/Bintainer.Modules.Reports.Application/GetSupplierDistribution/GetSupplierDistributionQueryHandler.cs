using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.Reports.Application.GetSupplierDistribution;

internal sealed class GetSupplierDistributionQueryHandler(
    IReportReadService reportReadService) : IQueryHandler<GetSupplierDistributionQuery, IReadOnlyCollection<SupplierDistributionResponse>>
{
    public async Task<Result<IReadOnlyCollection<SupplierDistributionResponse>>> Handle(GetSupplierDistributionQuery request, CancellationToken cancellationToken)
    {
        var rows = await reportReadService.GetSupplierDistributionAsync(cancellationToken);
        return Result.Success(rows);
    }
}
