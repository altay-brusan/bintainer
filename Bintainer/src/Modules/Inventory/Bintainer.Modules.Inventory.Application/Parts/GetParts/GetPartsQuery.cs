using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Parts.GetParts;

public sealed record GetPartsQuery(Guid? CategoryId) : IQuery<IReadOnlyCollection<PartSummaryResponse>>;

public sealed record PartSummaryResponse(
    Guid Id,
    string PartNumber,
    string ManufacturerPartNumber,
    string Description,
    string? ImageUrl,
    Guid? CategoryId,
    string? CategoryName,
    Guid? FootprintId,
    string? FootprintName,
    string? Tags);
