using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Parts.UpdatePart;

public sealed record UpdatePartCommand(
    Guid PartId,
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
    string? Tags) : ICommand;
