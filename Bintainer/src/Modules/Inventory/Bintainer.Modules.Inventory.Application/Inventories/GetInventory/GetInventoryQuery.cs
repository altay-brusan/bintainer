using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Inventories.GetInventory;

public sealed record GetInventoryQuery(Guid InventoryId) : IQuery<InventoryResponse>;

public sealed record InventoryResponse(Guid Id, string Name, Guid UserId);
