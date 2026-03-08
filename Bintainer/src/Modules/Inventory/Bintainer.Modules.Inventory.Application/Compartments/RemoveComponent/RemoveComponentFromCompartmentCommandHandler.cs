using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Compartments;

namespace Bintainer.Modules.Inventory.Application.Compartments.RemoveComponent;

internal sealed class RemoveComponentFromCompartmentCommandHandler(
    ICompartmentRepository compartmentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<RemoveComponentFromCompartmentCommand>
{
    public async Task<Result> Handle(RemoveComponentFromCompartmentCommand request, CancellationToken cancellationToken)
    {
        Compartment? compartment = await compartmentRepository.GetByIdAsync(request.CompartmentId, cancellationToken);

        if (compartment is null)
        {
            return Result.Failure(CompartmentErrors.NotFound(request.CompartmentId));
        }

        compartment.RemoveComponent();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
