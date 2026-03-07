using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Footprints;

namespace Bintainer.Modules.Inventory.Application.Footprints.DeleteFootprint;

internal sealed class DeleteFootprintCommandHandler(
    IFootprintRepository footprintRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<DeleteFootprintCommand>
{
    public async Task<Result> Handle(DeleteFootprintCommand request, CancellationToken cancellationToken)
    {
        Footprint? footprint = await footprintRepository.GetByIdAsync(request.FootprintId, cancellationToken);

        if (footprint is null)
        {
            return Result.Failure(FootprintErrors.NotFound(request.FootprintId));
        }

        footprintRepository.Remove(footprint);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
