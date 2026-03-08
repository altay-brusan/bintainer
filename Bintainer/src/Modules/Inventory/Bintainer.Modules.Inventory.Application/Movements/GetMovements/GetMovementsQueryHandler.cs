using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Application.Movements.GetMovements;

internal sealed class GetMovementsQueryHandler(
    IMovementReadService movementReadService) : IQueryHandler<GetMovementsQuery, MovementsPagedResponse>
{
    public async Task<Result<MovementsPagedResponse>> Handle(GetMovementsQuery request, CancellationToken cancellationToken)
    {
        var result = await movementReadService.GetPagedAsync(
            request.Action, request.ComponentId, request.Q,
            request.Page, request.PageSize, cancellationToken);
        return result;
    }
}
