using Bintainer.Modules.Inventory.Application.Reports.GetCategoryDistribution;
using Bintainer.Modules.Inventory.Application.Reports.GetLowStock;
using Bintainer.Modules.Inventory.Application.Reports.GetMovementTimeline;
using Bintainer.Modules.Inventory.Application.Reports.GetStorageUtilization;
using Bintainer.Modules.Inventory.Application.Reports.GetSummary;
using Bintainer.Modules.Inventory.Application.Reports.GetSupplierDistribution;
using Bintainer.Modules.Inventory.Application.Reports.GetTopComponents;

namespace Bintainer.Modules.Inventory.Application.Reports;

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
