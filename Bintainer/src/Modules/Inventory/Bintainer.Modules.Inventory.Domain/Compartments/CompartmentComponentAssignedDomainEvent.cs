using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Compartments;

public sealed class CompartmentComponentAssignedDomainEvent(Guid compartmentId, Guid componentId, int quantity) : DomainEvent
{
    public Guid CompartmentId { get; init; } = compartmentId;
    public Guid ComponentId { get; init; } = componentId;
    public int Quantity { get; init; } = quantity;
}
