using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Catalog.Application.Categories.GetCategories;

public sealed record GetCategoriesQuery() : IQuery<IReadOnlyCollection<CategoryResponse>>;

public sealed record CategoryResponse(
    Guid Id,
    string Name,
    Guid? ParentId,
    List<CategoryResponse> Children);
