using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.BomImports.GetBomHistory;

public sealed record GetBomHistoryQuery(int Page, int PageSize) : IQuery<BomHistoryPagedResponse>;

public sealed record BomHistoryPagedResponse(
    int TotalCount,
    int Page,
    int PageSize,
    List<BomHistoryItemResponse> Items);

public sealed record BomHistoryItemResponse(
    Guid Id,
    string FileName,
    int TotalLines,
    int MatchedCount,
    int NewCount,
    decimal TotalValue,
    Guid UserId,
    DateTime Date);
