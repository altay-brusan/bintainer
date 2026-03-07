using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.BomImports.ImportBom;

public sealed record ImportBomCommand(
    string FileName,
    List<BomLineItem> Lines) : ICommand<ImportBomResponse>;

public sealed record BomLineItem(
    string PartNumber,
    int Quantity,
    string? Description);

public sealed record ImportBomResponse(
    Guid ImportId,
    int TotalLines,
    int MatchedCount,
    int NewCount,
    decimal TotalValue);
