using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Domain.Components;

namespace Bintainer.Modules.Catalog.Application.Components.DeleteComponent;

internal sealed class DeleteComponentCommandHandler(
    IComponentRepository componentRepository,
    IActivityLogger activityLogger,
    ICurrentUserService currentUserService,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteComponentCommand>
{
    public async Task<Result> Handle(DeleteComponentCommand request, CancellationToken cancellationToken)
    {
        Component? component = await componentRepository.GetByIdAsync(request.ComponentId, cancellationToken);

        if (component is null)
        {
            return Result.Failure(ComponentErrors.NotFound(request.ComponentId));
        }

        component.Raise(new ComponentDeletedDomainEvent(component.Id));

        componentRepository.Remove(component);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await activityLogger.LogAsync(
            currentUserService.UserId,
            "ComponentDeleted",
            "Component",
            component.Id,
            ct: cancellationToken);

        return Result.Success();
    }
}
