using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Domain.Categories;
using Bintainer.Modules.Catalog.Domain.Components;
using Bintainer.Modules.Catalog.Domain.Footprints;

namespace Bintainer.Modules.Catalog.Application.Components.CreateComponent;

internal sealed class CreateComponentCommandHandler(
    IComponentRepository componentRepository,
    ICategoryRepository categoryRepository,
    IFootprintRepository footprintRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateComponentCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateComponentCommand request, CancellationToken cancellationToken)
    {
        if (request.CategoryId.HasValue)
        {
            var category = await categoryRepository.GetByIdAsync(request.CategoryId.Value, cancellationToken);
            if (category is null)
            {
                return Result.Failure<Guid>(CategoryErrors.NotFound(request.CategoryId.Value));
            }
        }

        if (request.FootprintId.HasValue)
        {
            var footprint = await footprintRepository.GetByIdAsync(request.FootprintId.Value, cancellationToken);
            if (footprint is null)
            {
                return Result.Failure<Guid>(FootprintErrors.NotFound(request.FootprintId.Value));
            }
        }

        var component = Component.Create(
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

        componentRepository.Insert(component);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return component.Id;
    }
}
