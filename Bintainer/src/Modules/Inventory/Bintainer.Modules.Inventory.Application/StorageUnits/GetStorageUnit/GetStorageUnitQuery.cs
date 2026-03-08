using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.StorageUnits.GetStorageUnit;

public sealed record GetStorageUnitQuery(Guid StorageUnitId) : IQuery<StorageUnitResponse>;

public sealed record StorageUnitResponse(
    Guid Id,
    string Name,
    int Columns,
    int Rows,
    int CompartmentCount,
    Guid InventoryId,
    List<BinResponse> Bins);

public sealed record BinResponse(
    Guid Id,
    int Column,
    int Row,
    bool IsActive,
    List<CompartmentResponse> Compartments);

public sealed record CompartmentResponse(
    Guid Id,
    int Index,
    string Label,
    Guid? ComponentId,
    string? ComponentPartNumber,
    int Quantity,
    bool IsActive);
