using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Reports.GetTopComponents;

public sealed record GetTopComponentsQuery(string? SortBy, int Limit) : IQuery<IReadOnlyCollection<TopComponentResponse>>;

public sealed record TopComponentResponse(
    Guid Id,
    string PartNumber,
    string Description,
    int TotalQuantity,
    decimal TotalValue);
