using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Components.GetComponents;

public sealed record GetComponentsQuery(Guid? CategoryId) : IQuery<IReadOnlyCollection<ComponentSummaryResponse>>;

public sealed record ComponentSummaryResponse(
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
    string? Manufacturer);
