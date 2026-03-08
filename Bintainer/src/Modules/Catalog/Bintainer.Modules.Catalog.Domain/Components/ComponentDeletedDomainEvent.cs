using Bintainer.Common.Domain;

namespace Bintainer.Modules.Catalog.Domain.Components;

public sealed class ComponentDeletedDomainEvent(Guid componentId) : DomainEvent
{
    public Guid ComponentId { get; init; } = componentId;
}
