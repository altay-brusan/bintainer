using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Reports.Application.GetSupplierDistribution;

public sealed record GetSupplierDistributionQuery() : IQuery<IReadOnlyCollection<SupplierDistributionResponse>>;

public sealed record SupplierDistributionResponse(
    string SupplierName,
    int ComponentCount);
