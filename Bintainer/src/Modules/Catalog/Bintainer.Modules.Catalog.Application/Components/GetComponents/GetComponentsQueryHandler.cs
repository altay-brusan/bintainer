using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.Catalog.Application.Components.GetComponents;

internal sealed class GetComponentsQueryHandler(
    IComponentReadService componentReadService) : IQueryHandler<GetComponentsQuery, IReadOnlyCollection<ComponentSummaryResponse>>
{
    public async Task<Result<IReadOnlyCollection<ComponentSummaryResponse>>> Handle(GetComponentsQuery request, CancellationToken cancellationToken)
    {
        var components = await componentReadService.GetByCategoryIdAsync(request.CategoryId, cancellationToken);
        return Result.Success(components);
    }
}
