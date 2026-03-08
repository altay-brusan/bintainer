using Bintainer.Common.Domain;

namespace Bintainer.Modules.Catalog.Domain.Categories;

public static class CategoryErrors
{
    public static Error NotFound(Guid categoryId) =>
        Error.NotFound("Categories.NotFound", $"The category with Id '{categoryId}' was not found.");

    public static Error HasChildren(Guid categoryId) =>
        Error.Conflict("Categories.HasChildren", $"The category with Id '{categoryId}' has child categories and cannot be deleted.");

    public static Error CircularReference(Guid categoryId) =>
        Error.Conflict("Categories.CircularReference", $"Setting this parent would create a circular reference for category '{categoryId}'.");
}
