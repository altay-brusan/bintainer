using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Movements;

public sealed class ComponentMovedDomainEvent(
    Guid componentId,
    Guid sourceCompartmentId,
    Guid destinationCompartmentId,
    int quantity) : DomainEvent
{
    public Guid ComponentId { get; init; } = componentId;
    public Guid SourceCompartmentId { get; init; } = sourceCompartmentId;
    public Guid DestinationCompartmentId { get; init; } = destinationCompartmentId;
    public int Quantity { get; init; } = quantity;
}
