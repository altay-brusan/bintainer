using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.Catalog.Application.Categories.GetCategories;

internal sealed class GetCategoriesQueryHandler(
    ICategoryReadService categoryReadService) : IQueryHandler<GetCategoriesQuery, IReadOnlyCollection<CategoryResponse>>
{
    public async Task<Result<IReadOnlyCollection<CategoryResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await categoryReadService.GetAllAsync(cancellationToken);
        return Result.Success(categories);
    }
}
