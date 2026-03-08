using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Domain.Footprints;

namespace Bintainer.Modules.Catalog.Application.Footprints.DeleteFootprint;

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

        footprint.Raise(new FootprintDeletedDomainEvent(footprint.Id, footprint.Name));

        footprintRepository.Remove(footprint);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
