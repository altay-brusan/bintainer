using Bintainer.Modules.Catalog.IntegrationEvents;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Compartments;
using MassTransit;

namespace Bintainer.Modules.Inventory.Infrastructure.Consumers;

internal sealed class ComponentDeletedIntegrationEventConsumer(
    ICompartmentRepository compartmentRepository,
    IUnitOfWork unitOfWork) : IConsumer<ComponentDeletedIntegrationEvent>
{
    public async Task Consume(ConsumeContext<ComponentDeletedIntegrationEvent> context)
    {
        List<Compartment> compartments = await compartmentRepository
            .GetByComponentIdAsync(context.Message.ComponentId, context.CancellationToken);

        foreach (Compartment compartment in compartments)
        {
            compartment.RemoveComponent();
        }

        await unitOfWork.SaveChangesAsync(context.CancellationToken);
    }
}
