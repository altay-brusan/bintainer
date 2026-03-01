using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Inventories.CreateInventory;

public sealed record CreateInventoryCommand(string Name, Guid UserId) : ICommand<Guid>;
