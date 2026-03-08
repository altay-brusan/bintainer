using Bintainer.Common.Application.EventBus;
using Bintainer.Modules.Catalog.Application.Components.DeleteComponent;
using Bintainer.Modules.Catalog.Domain.Components;
using Bintainer.Modules.Catalog.IntegrationEvents;

namespace Bintainer.Modules.Catalog.Application.UnitTests.Components.DeleteComponent;

public class ComponentDeletedDomainEventHandlerTests
{
    private readonly IEventBus _eventBus = Substitute.For<IEventBus>();
    private readonly ComponentDeletedDomainEventHandler _handler;

    public ComponentDeletedDomainEventHandlerTests()
    {
        _handler = new ComponentDeletedDomainEventHandler(_eventBus);
    }

    [Fact]
    public async Task Handle_PublishesIntegrationEvent()
    {
        var componentId = Guid.NewGuid();
        var domainEvent = new ComponentDeletedDomainEvent(componentId, "TEST-001");

        await _handler.Handle(domainEvent, CancellationToken.None);

        await _eventBus.Received(1).PublishAsync(
            Arg.Is<ComponentDeletedIntegrationEvent>(e => e.ComponentId == componentId),
            Arg.Any<CancellationToken>());
    }
}
