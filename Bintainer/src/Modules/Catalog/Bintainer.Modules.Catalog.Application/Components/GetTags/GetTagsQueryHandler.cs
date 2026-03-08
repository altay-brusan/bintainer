using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.Catalog.Application.Components.GetTags;

internal sealed class GetTagsQueryHandler(
    IComponentReadService componentReadService) : IQueryHandler<GetTagsQuery, IReadOnlyCollection<string>>
{
    public async Task<Result<IReadOnlyCollection<string>>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await componentReadService.GetTagsAsync(cancellationToken);
        return Result.Success(tags);
    }
}
