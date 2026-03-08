using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.Application.Abstractions;
using Bintainer.Modules.Catalog.Domain.Components;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Compartments;
using Bintainer.Modules.Inventory.Domain.Movements;

namespace Bintainer.Modules.Inventory.Application.Components.AdjustComponentQuantity;

internal sealed class AdjustComponentQuantityCommandHandler(
    ICatalogApi catalogApi,
    ICompartmentRepository compartmentRepository,
    IMovementRepository movementRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : ICommandHandler<AdjustComponentQuantityCommand>
{
    public async Task<Result> Handle(AdjustComponentQuantityCommand request, CancellationToken cancellationToken)
    {
        bool componentExists = await catalogApi.ComponentExistsAsync(request.ComponentId, cancellationToken);
        if (!componentExists)
        {
            return Result.Failure(ComponentErrors.NotFound(request.ComponentId));
        }

        Compartment? compartment = await compartmentRepository.GetByIdAsync(request.CompartmentId, cancellationToken);
        if (compartment is null)
        {
            return Result.Failure(CompartmentErrors.NotFound(request.CompartmentId));
        }

        if (!compartment.IsActive)
        {
            return Result.Failure(CompartmentErrors.Inactive);
        }

        int delta = request.Action switch
        {
            MovementAction.Added or MovementAction.Restocked => request.Quantity,
            MovementAction.Used => -request.Quantity,
            _ => 0
        };

        if (delta == 0)
        {
            return Result.Failure(MovementErrors.InvalidAction(request.Action));
        }

        if (!compartment.ComponentId.HasValue)
        {
            compartment.AssignComponent(request.ComponentId, request.Quantity);
        }
        else
        {
            compartment.AdjustQuantity(delta);
            if (compartment.Quantity == 0)
            {
                compartment.RemoveComponent();
            }
        }

        var movement = Movement.Create(
            request.ComponentId,
            request.Action,
            request.Quantity,
            request.CompartmentId,
            null,
            currentUserService.UserId,
            request.Notes);

        movementRepository.Insert(movement);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
