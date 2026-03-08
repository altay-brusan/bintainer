using Bintainer.Common.Application.EventBus;

namespace Bintainer.Modules.Catalog.IntegrationEvents;

public sealed class ComponentDeletedIntegrationEvent(
    Guid id,
    DateTime occurredOnUtc,
    Guid componentId) : IntegrationEvent(id, occurredOnUtc)
{
    public Guid ComponentId { get; init; } = componentId;
}
