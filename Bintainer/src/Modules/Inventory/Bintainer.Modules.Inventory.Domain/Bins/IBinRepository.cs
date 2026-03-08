namespace Bintainer.Modules.Inventory.Domain.Bins;

public interface IBinRepository
{
    Task<Bin?> GetByIdWithCompartmentsAsync(Guid id, CancellationToken cancellationToken = default);
}
