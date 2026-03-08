using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Compartments;

namespace Bintainer.Modules.Inventory.Application.Compartments.ActivateCompartment;

internal sealed class ActivateCompartmentCommandHandler(
    ICompartmentRepository compartmentRepository,
    IUnitOfWork unitOfWork,
    IActivityLogger activityLogger,
    ICurrentUserService currentUserService) : ICommandHandler<ActivateCompartmentCommand>
{
    public async Task<Result> Handle(ActivateCompartmentCommand request, CancellationToken cancellationToken)
    {
        Compartment? compartment = await compartmentRepository.GetByIdAsync(request.CompartmentId, cancellationToken);

        if (compartment is null)
        {
            return Result.Failure(CompartmentErrors.NotFound(request.CompartmentId));
        }

        compartment.Activate();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await activityLogger.LogAsync(
            currentUserService.UserId,
            "CompartmentActivated",
            "Compartment",
            compartment.Id,
            ct: cancellationToken);

        return Result.Success();
    }
}
