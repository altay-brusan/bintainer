using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Bins;

namespace Bintainer.Modules.Inventory.Application.Bins.ActivateBin;

internal sealed class ActivateBinCommandHandler(
    IBinRepository binRepository,
    IUnitOfWork unitOfWork) : ICommandHandler<ActivateBinCommand>
{
    public async Task<Result> Handle(ActivateBinCommand request, CancellationToken cancellationToken)
    {
        Bin? bin = await binRepository.GetByIdWithCompartmentsAsync(request.BinId, cancellationToken);

        if (bin is null)
        {
            return Result.Failure(BinErrors.NotFound(request.BinId));
        }

        bin.Activate();

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
