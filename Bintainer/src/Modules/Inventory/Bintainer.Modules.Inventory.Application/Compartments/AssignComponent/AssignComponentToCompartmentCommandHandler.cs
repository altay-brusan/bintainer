using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.IntegrationEvents;
using Bintainer.Modules.Inventory.Domain.Components;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Compartments;

namespace Bintainer.Modules.Inventory.Application.Compartments.AssignComponent;

internal sealed class AssignComponentToCompartmentCommandHandler(
    ICompartmentRepository compartmentRepository,
    ICatalogApi catalogApi,
    IUnitOfWork unitOfWork) : ICommandHandler<AssignComponentToCompartmentCommand>
{
    public async Task<Result> Handle(AssignComponentToCompartmentCommand request, CancellationToken cancellationToken)
    {
        Compartment? compartment = await compartmentRepository.GetByIdAsync(request.CompartmentId, cancellationToken);

        if (compartment is null)
        {
            return Result.Failure(CompartmentErrors.NotFound(request.CompartmentId));
        }

        if (!compartment.IsActive)
        {
            return Result.Failure(CompartmentErrors.Inactive);
        }

        bool componentExists = await catalogApi.ComponentExistsAsync(request.ComponentId, cancellationToken);

        if (!componentExists)
        {
            return Result.Failure(ComponentErrors.NotFound(request.ComponentId));
        }

        compartment.AssignComponent(request.ComponentId, request.Quantity);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
