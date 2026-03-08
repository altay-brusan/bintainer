namespace Bintainer.Modules.Catalog.Application.Abstractions;

public interface ICatalogApi
{
    Task<bool> ComponentExistsAsync(Guid componentId, CancellationToken ct = default);
}
