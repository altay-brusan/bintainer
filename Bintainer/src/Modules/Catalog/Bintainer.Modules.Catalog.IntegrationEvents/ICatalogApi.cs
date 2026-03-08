namespace Bintainer.Modules.Catalog.IntegrationEvents;

public interface ICatalogApi
{
    Task<bool> ComponentExistsAsync(Guid componentId, CancellationToken ct = default);
}
