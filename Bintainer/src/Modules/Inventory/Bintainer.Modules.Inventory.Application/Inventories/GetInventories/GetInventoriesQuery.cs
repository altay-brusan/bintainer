using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Inventories.GetInventories;

public sealed record GetInventoriesQuery(Guid UserId) : IQuery<IReadOnlyCollection<InventorySummaryResponse>>;

public sealed record InventorySummaryResponse(Guid Id, string Name, Guid UserId);
