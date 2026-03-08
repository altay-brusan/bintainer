using Bintainer.Common.Application.EventBus;
using Bintainer.Common.Application.Messaging;
using Bintainer.Modules.Catalog.Domain.Components;
using Bintainer.Modules.Catalog.IntegrationEvents;

namespace Bintainer.Modules.Catalog.Application.Components.DeleteComponent;

internal sealed class ComponentDeletedDomainEventHandler(IEventBus eventBus)
    : IDomainEventHandler<ComponentDeletedDomainEvent>
{
    public async Task Handle(ComponentDeletedDomainEvent notification, CancellationToken cancellationToken)
    {
        await eventBus.PublishAsync(
            new ComponentDeletedIntegrationEvent(
                notification.Id,
                notification.OccurredOnUtc,
                notification.ComponentId),
            cancellationToken);
    }
}
