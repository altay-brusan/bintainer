using Bintainer.Modules.Catalog.IntegrationEvents;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Compartments;
using Bintainer.Modules.Inventory.Domain.StorageUnits;
using Bintainer.Modules.Inventory.Infrastructure.Consumers;
using MassTransit;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Consumers;

public class ComponentDeletedIntegrationEventConsumerTests
{
    private readonly ICompartmentRepository _compartmentRepository = Substitute.For<ICompartmentRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly ComponentDeletedIntegrationEventConsumer _consumer;

    public ComponentDeletedIntegrationEventConsumerTests()
    {
        _consumer = new ComponentDeletedIntegrationEventConsumer(_compartmentRepository, _unitOfWork);
    }

    private static Compartment CreateCompartmentWithComponent(Guid componentId)
    {
        var unit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());
        var compartment = unit.Bins.First().Compartments.First();
        compartment.AssignComponent(componentId, 5);
        return compartment;
    }

    [Fact]
    public async Task Consume_RemovesComponentFromCompartments()
    {
        var componentId = Guid.NewGuid();
        var compartment1 = CreateCompartmentWithComponent(componentId);
        var compartment2 = CreateCompartmentWithComponent(componentId);

        _compartmentRepository.GetByComponentIdAsync(componentId, Arg.Any<CancellationToken>())
            .Returns(new List<Compartment> { compartment1, compartment2 });

        var integrationEvent = new ComponentDeletedIntegrationEvent(
            Guid.NewGuid(), DateTime.UtcNow, componentId);

        var context = Substitute.For<ConsumeContext<ComponentDeletedIntegrationEvent>>();
        context.Message.Returns(integrationEvent);
        context.CancellationToken.Returns(CancellationToken.None);

        await _consumer.Consume(context);

        compartment1.ComponentId.Should().BeNull();
        compartment1.Quantity.Should().Be(0);
        compartment2.ComponentId.Should().BeNull();
        compartment2.Quantity.Should().Be(0);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Consume_NoCompartments_StillSaves()
    {
        var componentId = Guid.NewGuid();

        _compartmentRepository.GetByComponentIdAsync(componentId, Arg.Any<CancellationToken>())
            .Returns(new List<Compartment>());

        var integrationEvent = new ComponentDeletedIntegrationEvent(
            Guid.NewGuid(), DateTime.UtcNow, componentId);

        var context = Substitute.For<ConsumeContext<ComponentDeletedIntegrationEvent>>();
        context.Message.Returns(integrationEvent);
        context.CancellationToken.Returns(CancellationToken.None);

        await _consumer.Consume(context);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
