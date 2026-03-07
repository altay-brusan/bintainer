using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Parts.GetPart;

public sealed record GetPartQuery(Guid PartId) : IQuery<PartResponse>;

public sealed record PartResponse(
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
    string? Tags);
