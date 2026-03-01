using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Inventories;

public static class InventoryErrors
{
    public static Error NotFound(Guid inventoryId) =>
        Error.NotFound("Inventories.NotFound", $"The inventory with Id '{inventoryId}' was not found.");
}
