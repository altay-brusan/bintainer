using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.StorageUnits.GetStorageUnits;

public sealed record GetStorageUnitsQuery(Guid InventoryId) : IQuery<IReadOnlyCollection<StorageUnitSummaryResponse>>;

public sealed record StorageUnitSummaryResponse(
    Guid Id,
    string Name,
    int Columns,
    int Rows,
    int CompartmentCount,
    Guid InventoryId);
