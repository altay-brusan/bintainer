using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Reports.Application.GetTopComponents;

public sealed record GetTopComponentsQuery(string? SortBy, int Limit) : IQuery<IReadOnlyCollection<TopComponentResponse>>;

public sealed record TopComponentResponse(
    Guid Id,
    string PartNumber,
    string Description,
    int TotalQuantity,
    decimal TotalValue);
