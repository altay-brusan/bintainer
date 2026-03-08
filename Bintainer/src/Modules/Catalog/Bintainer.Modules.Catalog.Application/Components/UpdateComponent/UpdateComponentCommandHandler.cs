using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Domain.Components;

namespace Bintainer.Modules.Catalog.Application.Components.UpdateComponent;

internal sealed class UpdateComponentCommandHandler(
    IComponentRepository componentRepository,
    IActivityLogger activityLogger,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdateComponentCommand>
{
    public async Task<Result> Handle(UpdateComponentCommand request, CancellationToken cancellationToken)
    {
        Component? component = await componentRepository.GetByIdAsync(request.ComponentId, cancellationToken);

        if (component is null)
        {
            return Result.Failure(ComponentErrors.NotFound(request.ComponentId));
        }

        component.Update(
            request.PartNumber,
            request.ManufacturerPartNumber,
            request.Description,
            request.DetailedDescription,
            request.ImageUrl,
            request.Url,
            request.Provider,
            request.ProviderPartNumber,
            request.CategoryId,
            request.FootprintId,
            request.Attributes,
            request.Tags,
            request.UnitPrice,
            request.Manufacturer,
            request.LowStockThreshold);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await activityLogger.LogAsync(
            currentUserService.UserId,
            "ComponentUpdated",
            "Component",
            component.Id,
            request.PartNumber,
            ct: cancellationToken);

        return Result.Success();
    }
}
