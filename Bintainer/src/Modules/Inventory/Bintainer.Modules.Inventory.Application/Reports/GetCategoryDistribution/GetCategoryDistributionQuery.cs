using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Reports.GetCategoryDistribution;

public sealed record GetCategoryDistributionQuery() : IQuery<IReadOnlyCollection<CategoryDistributionResponse>>;

public sealed record CategoryDistributionResponse(
    string CategoryName,
    int ComponentCount,
    decimal TotalValue);
