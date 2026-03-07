using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Footprints;

namespace Bintainer.Modules.Inventory.Application.Footprints.UpdateFootprint;

internal sealed class UpdateFootprintCommandHandler(
    IFootprintRepository footprintRepository,
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

        return Result.Success();
    }
}
