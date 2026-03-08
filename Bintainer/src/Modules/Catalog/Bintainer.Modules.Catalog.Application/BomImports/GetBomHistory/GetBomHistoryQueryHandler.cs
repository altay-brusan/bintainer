using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.Catalog.Application.BomImports.GetBomHistory;

internal sealed class GetBomHistoryQueryHandler(
    IBomImportReadService bomImportReadService) : IQueryHandler<GetBomHistoryQuery, BomHistoryPagedResponse>
{
    public async Task<Result<BomHistoryPagedResponse>> Handle(GetBomHistoryQuery request, CancellationToken cancellationToken)
    {
        var result = await bomImportReadService.GetPagedAsync(request.Page, request.PageSize, cancellationToken);
        return result;
    }
}
