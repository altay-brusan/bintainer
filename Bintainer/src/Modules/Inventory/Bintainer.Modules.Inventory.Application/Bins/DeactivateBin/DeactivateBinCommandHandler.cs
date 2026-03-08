using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Bins;

namespace Bintainer.Modules.Inventory.Application.Bins.DeactivateBin;

internal sealed class DeactivateBinCommandHandler(
    IBinRepository binRepository,
    IUnitOfWork unitOfWork,
    IActivityLogger activityLogger,
    ICurrentUserService currentUserService) : ICommandHandler<DeactivateBinCommand>
{
    public async Task<Result> Handle(DeactivateBinCommand request, CancellationToken cancellationToken)
    {
        Bin? bin = await binRepository.GetByIdWithCompartmentsAsync(request.BinId, cancellationToken);

        if (bin is null)
        {
            return Result.Failure(BinErrors.NotFound(request.BinId));
        }

        Result result = bin.Deactivate();

        if (result.IsFailure)
        {
            return result;
        }

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await activityLogger.LogAsync(
            currentUserService.UserId,
            "BinDeactivated",
            "Bin",
            bin.Id,
            ct: cancellationToken);

        return Result.Success();
    }
}
