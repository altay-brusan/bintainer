namespace Bintainer.Modules.Inventory.Domain.Compartments;

public interface ICompartmentRepository
{
    Task<Compartment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<List<Compartment>> GetByComponentIdAsync(Guid componentId, CancellationToken cancellationToken = default);
}
