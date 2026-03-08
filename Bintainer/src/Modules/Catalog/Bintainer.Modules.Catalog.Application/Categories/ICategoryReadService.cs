using Bintainer.Modules.Catalog.Application.Categories.GetCategories;

namespace Bintainer.Modules.Catalog.Application.Categories;

public interface ICategoryReadService
{
    Task<IReadOnlyCollection<CategoryResponse>> GetAllAsync(CancellationToken ct);
}
