using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Components;

namespace Bintainer.Modules.Inventory.Application.Components.DeleteComponent;

internal sealed class DeleteComponentCommandHandler(
    IComponentRepository componentRepository,
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

        return Result.Success();
    }
}
