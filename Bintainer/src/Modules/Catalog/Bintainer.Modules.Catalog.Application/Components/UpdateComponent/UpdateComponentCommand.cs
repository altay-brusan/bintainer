using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Catalog.Application.Components.UpdateComponent;

public sealed record UpdateComponentCommand(
    Guid ComponentId,
    string PartNumber,
    string ManufacturerPartNumber,
    string Description,
    string? DetailedDescription,
    string? ImageUrl,
    string? Url,
    string? Provider,
    string? ProviderPartNumber,
    Guid? CategoryId,
    Guid? FootprintId,
    Dictionary<string, string>? Attributes,
    string? Tags,
    decimal? UnitPrice,
    string? Manufacturer,
    int LowStockThreshold) : ICommand;
