using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Compartments;
using Bintainer.Modules.Inventory.Domain.Components;

namespace Bintainer.Modules.Inventory.Application.Compartments.AssignComponent;

internal sealed class AssignComponentToCompartmentCommandHandler(
    ICompartmentRepository compartmentRepository,
    IComponentRepository componentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<AssignComponentToCompartmentCommand>
{
    public async Task<Result> Handle(AssignComponentToCompartmentCommand request, CancellationToken cancellationToken)
    {
        Compartment? compartment = await compartmentRepository.GetByIdAsync(request.CompartmentId, cancellationToken);

        if (compartment is null)
        {
            return Result.Failure(CompartmentErrors.NotFound(request.CompartmentId));
        }

        Component? component = await componentRepository.GetByIdAsync(request.ComponentId, cancellationToken);

        if (component is null)
        {
            return Result.Failure(ComponentErrors.NotFound(request.ComponentId));
        }

        compartment.AssignComponent(request.ComponentId, request.Quantity);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
