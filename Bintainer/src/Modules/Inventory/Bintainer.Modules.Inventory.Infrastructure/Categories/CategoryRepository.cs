using Bintainer.Modules.Inventory.Domain.Categories;
using Bintainer.Modules.Inventory.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;

namespace Bintainer.Modules.Inventory.Infrastructure.Categories;

internal sealed class CategoryRepository(InventoryDbContext dbContext) : ICategoryRepository
{
    public async Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Categories.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Category?> GetByIdWithChildrenAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Categories
            .Include(c => c.Children)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<bool> HasChildrenAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await dbContext.Categories.AnyAsync(c => c.ParentId == id, cancellationToken);
    }

    public void Insert(Category category)
    {
        dbContext.Categories.Add(category);
    }

    public void Remove(Category category)
    {
        dbContext.Categories.Remove(category);
    }
}
