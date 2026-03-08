using Bintainer.Modules.Reports.Application.GetCategoryDistribution;
using Bintainer.Modules.Reports.Application.GetLowStock;
using Bintainer.Modules.Reports.Application.GetMovementTimeline;
using Bintainer.Modules.Reports.Application.GetStorageUtilization;
using Bintainer.Modules.Reports.Application.GetSummary;
using Bintainer.Modules.Reports.Application.GetSupplierDistribution;
using Bintainer.Modules.Reports.Application.GetTopComponents;

namespace Bintainer.Modules.Reports.Application;

public interface IReportReadService
{
    Task<SummaryResponse> GetSummaryAsync(CancellationToken ct);
    Task<IReadOnlyCollection<TopComponentResponse>> GetTopComponentsAsync(string? sortBy, int limit, CancellationToken ct);
    Task<IReadOnlyCollection<LowStockResponse>> GetLowStockAsync(CancellationToken ct);
    Task<IReadOnlyCollection<StorageUtilizationResponse>> GetStorageUtilizationAsync(CancellationToken ct);
    Task<IReadOnlyCollection<MovementTimelineResponse>> GetMovementTimelineAsync(int days, CancellationToken ct);
    Task<IReadOnlyCollection<SupplierDistributionResponse>> GetSupplierDistributionAsync(CancellationToken ct);
    Task<IReadOnlyCollection<CategoryDistributionResponse>> GetCategoryDistributionAsync(CancellationToken ct);
}
