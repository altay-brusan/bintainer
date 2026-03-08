namespace Bintainer.Modules.Catalog.Domain.Components;

public interface IComponentRepository
{
    Task<Component?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Component?> GetByPartNumberAsync(string partNumber, CancellationToken cancellationToken = default);
    void Insert(Component component);
    void Remove(Component component);
}
