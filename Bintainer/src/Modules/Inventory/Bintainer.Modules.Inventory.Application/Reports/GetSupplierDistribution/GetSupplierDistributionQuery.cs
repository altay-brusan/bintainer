using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Reports.GetSupplierDistribution;

public sealed record GetSupplierDistributionQuery() : IQuery<IReadOnlyCollection<SupplierDistributionResponse>>;

public sealed record SupplierDistributionResponse(
    string SupplierName,
    int ComponentCount);
