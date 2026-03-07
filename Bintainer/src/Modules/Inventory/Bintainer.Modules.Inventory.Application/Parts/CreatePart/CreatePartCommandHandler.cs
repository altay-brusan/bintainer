using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Categories;
using Bintainer.Modules.Inventory.Domain.Footprints;
using Bintainer.Modules.Inventory.Domain.Parts;

namespace Bintainer.Modules.Inventory.Application.Parts.CreatePart;

internal sealed class CreatePartCommandHandler(
    IPartRepository partRepository,
    ICategoryRepository categoryRepository,
    IFootprintRepository footprintRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreatePartCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreatePartCommand request, CancellationToken cancellationToken)
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

        var part = Part.Create(
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
            request.Tags);

        partRepository.Insert(part);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return part.Id;
    }
}
