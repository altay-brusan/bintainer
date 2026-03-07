using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Components.SearchComponents;

public sealed record SearchComponentsQuery(
    string? Q,
    Guid? CategoryId,
    string? Provider,
    string? Tag,
    Guid? FootprintId,
    int Page,
    int PageSize) : IQuery<SearchComponentsPagedResponse>;

public sealed record SearchComponentsPagedResponse(
    int TotalCount,
    int Page,
    int PageSize,
    List<SearchComponentItemResponse> Items);

public sealed record SearchComponentItemResponse(
    Guid Id,
    string PartNumber,
    string ManufacturerPartNumber,
    string Description,
    string? ImageUrl,
    Guid? CategoryId,
    string? CategoryName,
    Guid? FootprintId,
    string? FootprintName,
    string? Tags,
    decimal? UnitPrice,
    string? Manufacturer,
    int TotalQuantity);
