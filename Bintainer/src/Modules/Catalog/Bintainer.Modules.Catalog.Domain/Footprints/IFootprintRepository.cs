namespace Bintainer.Modules.Catalog.Domain.Footprints;

public interface IFootprintRepository
{
    Task<Footprint?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    void Insert(Footprint footprint);
    void Remove(Footprint footprint);
}
