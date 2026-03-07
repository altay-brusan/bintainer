using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Compartments;

namespace Bintainer.Modules.Inventory.Application.Compartments.UpdateLabel;

internal sealed class UpdateCompartmentLabelCommandHandler(
    ICompartmentRepository compartmentRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateCompartmentLabelCommand>
{
    public async Task<Result> Handle(UpdateCompartmentLabelCommand request, CancellationToken cancellationToken)
    {
        Compartment? compartment = await compartmentRepository.GetByIdAsync(request.CompartmentId, cancellationToken);

        if (compartment is null)
        {
            return Result.Failure(CompartmentErrors.NotFound(request.CompartmentId));
        }

        compartment.UpdateLabel(request.Label);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
