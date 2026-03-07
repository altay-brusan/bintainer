using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Components.GetComponent;

public sealed record GetComponentQuery(Guid ComponentId) : IQuery<ComponentResponse>;

public sealed record ComponentResponse(
    Guid Id,
    string PartNumber,
    string ManufacturerPartNumber,
    string Description,
    string? DetailedDescription,
    string? ImageUrl,
    string? Url,
    string? Provider,
    string? ProviderPartNumber,
    Guid? CategoryId,
    string? CategoryName,
    Guid? FootprintId,
    string? FootprintName,
    Dictionary<string, string> Attributes,
    string? Tags,
    decimal? UnitPrice,
    string? Manufacturer,
    int LowStockThreshold,
    List<ComponentLocationResponse> Locations);

public sealed record ComponentLocationResponse(
    Guid CompartmentId,
    string Label,
    int Quantity,
    Guid BinId,
    Guid StorageUnitId,
    string StorageUnitName);
