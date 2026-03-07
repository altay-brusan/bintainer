using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Parts;

namespace Bintainer.Modules.Inventory.Application.Parts.UpdatePart;

internal sealed class UpdatePartCommandHandler(
    IPartRepository partRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<UpdatePartCommand>
{
    public async Task<Result> Handle(UpdatePartCommand request, CancellationToken cancellationToken)
    {
        Part? part = await partRepository.GetByIdAsync(request.PartId, cancellationToken);

        if (part is null)
        {
            return Result.Failure(PartErrors.NotFound(request.PartId));
        }

        part.Update(
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

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
