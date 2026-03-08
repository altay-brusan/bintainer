using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Compartments;

public sealed class ComponentQuantityAdjustedDomainEvent(
    Guid compartmentId,
    Guid componentId,
    string action,
    int quantity) : DomainEvent
{
    public Guid CompartmentId { get; init; } = compartmentId;
    public Guid ComponentId { get; init; } = componentId;
    public string Action { get; init; } = action;
    public int Quantity { get; init; } = quantity;
}
