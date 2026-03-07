using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Footprints;

namespace Bintainer.Modules.Inventory.Application.Footprints.CreateFootprint;

internal sealed class CreateFootprintCommandHandler(
    IFootprintRepository footprintRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<CreateFootprintCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateFootprintCommand request, CancellationToken cancellationToken)
    {
        var footprint = Footprint.Create(request.Name);

        footprintRepository.Insert(footprint);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return footprint.Id;
    }
}
