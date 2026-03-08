using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Domain.Footprints;

namespace Bintainer.Modules.Catalog.Application.Footprints.DeleteFootprint;

internal sealed class DeleteFootprintCommandHandler(
    IFootprintRepository footprintRepository,
    IActivityLogger activityLogger,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteFootprintCommand>
{
    public async Task<Result> Handle(DeleteFootprintCommand request, CancellationToken cancellationToken)
    {
        Footprint? footprint = await footprintRepository.GetByIdAsync(request.FootprintId, cancellationToken);

        if (footprint is null)
        {
            return Result.Failure(FootprintErrors.NotFound(request.FootprintId));
        }

        footprintRepository.Remove(footprint);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await activityLogger.LogAsync(
            currentUserService.UserId,
            "FootprintDeleted",
            "Footprint",
            footprint.Id,
            ct: cancellationToken);

        return Result.Success();
    }
}
