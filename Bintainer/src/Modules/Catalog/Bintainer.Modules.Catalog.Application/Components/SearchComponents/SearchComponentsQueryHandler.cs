using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.Catalog.Application.Components.SearchComponents;

internal sealed class SearchComponentsQueryHandler(
    IComponentReadService componentReadService) : IQueryHandler<SearchComponentsQuery, SearchComponentsPagedResponse>
{
    public async Task<Result<SearchComponentsPagedResponse>> Handle(SearchComponentsQuery request, CancellationToken cancellationToken)
    {
        var result = await componentReadService.SearchAsync(
            request.Q, request.CategoryId, request.Provider, request.Tag,
            request.FootprintId, request.Page, request.PageSize, cancellationToken);
        return result;
    }
}
