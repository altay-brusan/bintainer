using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.Reports.Application.GetStorageUtilization;

internal sealed class GetStorageUtilizationQueryHandler(
    IReportReadService reportReadService) : IQueryHandler<GetStorageUtilizationQuery, IReadOnlyCollection<StorageUtilizationResponse>>
{
    public async Task<Result<IReadOnlyCollection<StorageUtilizationResponse>>> Handle(GetStorageUtilizationQuery request, CancellationToken cancellationToken)
    {
        var rows = await reportReadService.GetStorageUtilizationAsync(cancellationToken);
        return Result.Success(rows);
    }
}
