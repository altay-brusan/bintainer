using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Reports.GetSummary;

public sealed record GetSummaryQuery() : IQuery<SummaryResponse>;

public sealed record SummaryResponse(
    int TotalComponents,
    int TotalCategories,
    int TotalStorageUnits,
    int OccupiedCompartments,
    int TotalQuantity,
    decimal TotalValue,
    int RecentMovements);
