using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Compartments;

public sealed class CompartmentLabelUpdatedDomainEvent(Guid compartmentId, string label) : DomainEvent
{
    public Guid CompartmentId { get; init; } = compartmentId;
    public string Label { get; init; } = label;
}
