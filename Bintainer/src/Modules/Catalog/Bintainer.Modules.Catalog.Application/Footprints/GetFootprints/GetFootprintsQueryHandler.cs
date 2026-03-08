using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.Catalog.Application.Footprints.GetFootprints;

internal sealed class GetFootprintsQueryHandler(
    IFootprintReadService footprintReadService) : IQueryHandler<GetFootprintsQuery, IReadOnlyCollection<FootprintResponse>>
{
    public async Task<Result<IReadOnlyCollection<FootprintResponse>>> Handle(GetFootprintsQuery request, CancellationToken cancellationToken)
    {
        var footprints = await footprintReadService.GetAllAsync(cancellationToken);
        return Result.Success(footprints);
    }
}
