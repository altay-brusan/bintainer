namespace Bintainer.Modules.Catalog.Domain.Categories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Category?> GetByIdWithChildrenAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> HasChildrenAsync(Guid id, CancellationToken cancellationToken = default);
    void Insert(Category category);
    void Remove(Category category);
}
