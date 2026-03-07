using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Components;

public sealed class ComponentCreatedDomainEvent(Guid componentId) : DomainEvent
{
    public Guid ComponentId { get; init; } = componentId;
}
