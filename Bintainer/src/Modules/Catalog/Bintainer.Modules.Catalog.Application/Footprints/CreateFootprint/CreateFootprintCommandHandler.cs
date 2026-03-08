using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Domain.Footprints;

namespace Bintainer.Modules.Catalog.Application.Footprints.CreateFootprint;

internal sealed class CreateFootprintCommandHandler(
    IFootprintRepository footprintRepository,
    IActivityLogger activityLogger,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateFootprintCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateFootprintCommand request, CancellationToken cancellationToken)
    {
        var footprint = Footprint.Create(request.Name);

        footprintRepository.Insert(footprint);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await activityLogger.LogAsync(
            currentUserService.UserId,
            "FootprintCreated",
            "Footprint",
            footprint.Id,
            request.Name,
            ct: cancellationToken);

        return footprint.Id;
    }
}
