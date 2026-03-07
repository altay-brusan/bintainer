using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Components;

namespace Bintainer.Modules.Inventory.Application.Components.UpdateComponent;

internal sealed class UpdateComponentCommandHandler(
    IComponentRepository componentRepository,
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

        return Result.Success();
    }
}
