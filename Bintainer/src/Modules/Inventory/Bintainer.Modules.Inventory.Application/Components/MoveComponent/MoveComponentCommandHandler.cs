using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.IntegrationEvents;
using Bintainer.Modules.Inventory.Domain.Components;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Compartments;
using Bintainer.Modules.Inventory.Domain.Movements;

namespace Bintainer.Modules.Inventory.Application.Components.MoveComponent;

internal sealed class MoveComponentCommandHandler(
    ICatalogApi catalogApi,
    ICompartmentRepository compartmentRepository,
    IMovementRepository movementRepository,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork,
    IActivityLogger activityLogger) : ICommandHandler<MoveComponentCommand>
{
    public async Task<Result> Handle(MoveComponentCommand request, CancellationToken cancellationToken)
    {
        bool componentExists = await catalogApi.ComponentExistsAsync(request.ComponentId, cancellationToken);
        if (!componentExists)
        {
            return Result.Failure(ComponentErrors.NotFound(request.ComponentId));
        }

        Compartment? source = await compartmentRepository.GetByIdAsync(request.SourceCompartmentId, cancellationToken);
        if (source is null)
        {
            return Result.Failure(CompartmentErrors.NotFound(request.SourceCompartmentId));
        }

        if (!source.IsActive)
        {
            return Result.Failure(CompartmentErrors.Inactive);
        }

        Compartment? destination = await compartmentRepository.GetByIdAsync(request.DestinationCompartmentId, cancellationToken);
        if (destination is null)
        {
            return Result.Failure(CompartmentErrors.NotFound(request.DestinationCompartmentId));
        }

        if (!destination.IsActive)
        {
            return Result.Failure(CompartmentErrors.Inactive);
        }

        if (source.Quantity < request.Quantity)
        {
            return Result.Failure(MovementErrors.InsufficientQuantity());
        }

        // Reduce source
        source.AdjustQuantity(-request.Quantity);
        if (source.Quantity == 0)
        {
            source.RemoveComponent();
        }

        // Add to destination
        if (destination.ComponentId.HasValue && destination.ComponentId == request.ComponentId)
        {
            destination.AdjustQuantity(request.Quantity);
        }
        else
        {
            destination.AssignComponent(request.ComponentId, request.Quantity);
        }

        var movement = Movement.Create(
            request.ComponentId,
            MovementAction.Moved,
            request.Quantity,
            request.DestinationCompartmentId,
            request.SourceCompartmentId,
            currentUserService.UserId,
            null);

        movementRepository.Insert(movement);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await activityLogger.LogAsync(
            currentUserService.UserId,
            "ComponentMoved",
            "Component",
            request.ComponentId,
            details: new { request.SourceCompartmentId, request.DestinationCompartmentId, request.Quantity },
            ct: cancellationToken);

        return Result.Success();
    }
}
