using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Domain.Footprints;

namespace Bintainer.Modules.Catalog.Application.Footprints.UpdateFootprint;

internal sealed class UpdateFootprintCommandHandler(
    IFootprintRepository footprintRepository,
    IActivityLogger activityLogger,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateFootprintCommand>
{
    public async Task<Result> Handle(UpdateFootprintCommand request, CancellationToken cancellationToken)
    {
        Footprint? footprint = await footprintRepository.GetByIdAsync(request.FootprintId, cancellationToken);

        if (footprint is null)
        {
            return Result.Failure(FootprintErrors.NotFound(request.FootprintId));
        }

        footprint.Update(request.Name);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await activityLogger.LogAsync(
            currentUserService.UserId,
            "FootprintUpdated",
            "Footprint",
            footprint.Id,
            request.Name,
            ct: cancellationToken);

        return Result.Success();
    }
}
