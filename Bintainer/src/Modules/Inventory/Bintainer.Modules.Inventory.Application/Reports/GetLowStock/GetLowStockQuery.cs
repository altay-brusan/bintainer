using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Reports.GetLowStock;

public sealed record GetLowStockQuery() : IQuery<IReadOnlyCollection<LowStockResponse>>;

public sealed record LowStockResponse(
    Guid Id,
    string PartNumber,
    string Description,
    int TotalQuantity,
    int LowStockThreshold);
