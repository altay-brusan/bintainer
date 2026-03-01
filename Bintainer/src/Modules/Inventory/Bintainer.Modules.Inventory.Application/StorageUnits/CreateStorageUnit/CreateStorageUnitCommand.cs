using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.StorageUnits.CreateStorageUnit;

public sealed record CreateStorageUnitCommand(
    string Name,
    int Columns,
    int Rows,
    int CompartmentCount,
    Guid InventoryId) : ICommand<Guid>;
