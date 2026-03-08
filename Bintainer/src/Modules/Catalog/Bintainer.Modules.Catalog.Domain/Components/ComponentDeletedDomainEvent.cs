using Bintainer.Common.Domain;

namespace Bintainer.Modules.Catalog.Domain.Components;

public sealed class ComponentDeletedDomainEvent(Guid componentId, string partNumber) : DomainEvent
{
    public Guid ComponentId { get; init; } = componentId;
    public string PartNumber { get; init; } = partNumber;
}
