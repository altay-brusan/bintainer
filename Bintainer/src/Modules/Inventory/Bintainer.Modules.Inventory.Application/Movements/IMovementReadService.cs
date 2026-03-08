using Bintainer.Modules.Inventory.Application.Movements.GetMovements;

namespace Bintainer.Modules.Inventory.Application.Movements;

public interface IMovementReadService
{
    Task<MovementsPagedResponse> GetPagedAsync(string? action, Guid? componentId, string? q, int page, int pageSize, CancellationToken ct);
}
