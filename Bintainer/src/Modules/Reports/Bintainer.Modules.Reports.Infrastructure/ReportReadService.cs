using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Modules.Reports.Application;
using Bintainer.Modules.Reports.Application.GetCategoryDistribution;
using Bintainer.Modules.Reports.Application.GetLowStock;
using Bintainer.Modules.Reports.Application.GetMovementTimeline;
using Bintainer.Modules.Reports.Application.GetStorageUtilization;
using Bintainer.Modules.Reports.Application.GetSummary;
using Bintainer.Modules.Reports.Application.GetSupplierDistribution;
using Bintainer.Modules.Reports.Application.GetTopComponents;
using Dapper;

namespace Bintainer.Modules.Reports.Infrastructure;

internal sealed class ReportReadService(IDbConnectionFactory dbConnectionFactory) : IReportReadService
{
    public async Task<SummaryResponse> GetSummaryAsync(CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT
                (SELECT COUNT(*)::int FROM catalog.components) AS TotalComponents,
                (SELECT COUNT(*)::int FROM catalog.categories) AS TotalCategories,
                (SELECT COUNT(*)::int FROM inventory.storage_units) AS TotalStorageUnits,
                (SELECT COUNT(*)::int FROM inventory.compartments WHERE component_id IS NOT NULL) AS OccupiedCompartments,
                (SELECT COALESCE(SUM(quantity), 0)::int FROM inventory.compartments WHERE component_id IS NOT NULL) AS TotalQuantity,
                (SELECT COALESCE(SUM(c.quantity * COALESCE(p.unit_price, 0)), 0)
                 FROM inventory.compartments c
                 JOIN catalog.components p ON p.id = c.component_id
                 WHERE c.component_id IS NOT NULL) AS TotalValue,
                (SELECT COUNT(*)::int FROM inventory.movements WHERE date >= NOW() - INTERVAL '30 days') AS RecentMovements
            """;

        return await connection.QuerySingleAsync<SummaryResponse>(sql);
    }

    public async Task<IReadOnlyCollection<TopComponentResponse>> GetTopComponentsAsync(string? sortBy, int limit, CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        limit = limit < 1 ? 10 : limit > 50 ? 50 : limit;
        var orderBy = sortBy?.ToLowerInvariant() == "value" ? "TotalValue" : "TotalQuantity";

        var sql = $"""
            SELECT
                p.id AS Id,
                p.part_number AS PartNumber,
                p.description AS Description,
                COALESCE(SUM(c.quantity), 0)::int AS TotalQuantity,
                COALESCE(SUM(c.quantity * COALESCE(p.unit_price, 0)), 0) AS TotalValue
            FROM catalog.components p
            LEFT JOIN inventory.compartments c ON c.component_id = p.id
            GROUP BY p.id, p.part_number, p.description
            ORDER BY {orderBy} DESC
            LIMIT @Limit
            """;

        var rows = (await connection.QueryAsync<TopComponentResponse>(sql, new { Limit = limit })).ToList();
        return rows;
    }

    public async Task<IReadOnlyCollection<LowStockResponse>> GetLowStockAsync(CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT
                p.id AS Id,
                p.part_number AS PartNumber,
                p.description AS Description,
                COALESCE(SUM(c.quantity), 0)::int AS TotalQuantity,
                p.low_stock_threshold AS LowStockThreshold
            FROM catalog.components p
            LEFT JOIN inventory.compartments c ON c.component_id = p.id
            WHERE p.low_stock_threshold > 0
            GROUP BY p.id, p.part_number, p.description, p.low_stock_threshold
            HAVING COALESCE(SUM(c.quantity), 0) <= p.low_stock_threshold
            ORDER BY p.part_number
            """;

        var rows = (await connection.QueryAsync<LowStockResponse>(sql)).ToList();
        return rows;
    }

    public async Task<IReadOnlyCollection<StorageUtilizationResponse>> GetStorageUtilizationAsync(CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT
                su.id AS StorageUnitId,
                su.name AS StorageUnitName,
                COUNT(c.id)::int AS TotalCompartments,
                COUNT(c.component_id)::int AS OccupiedCompartments
            FROM inventory.storage_units su
            JOIN inventory.bins b ON b.storage_unit_id = su.id
            JOIN inventory.compartments c ON c.bin_id = b.id
            GROUP BY su.id, su.name
            ORDER BY su.name
            """;

        var rows = (await connection.QueryAsync<StorageUtilizationResponse>(sql)).ToList();
        return rows;
    }

    public async Task<IReadOnlyCollection<MovementTimelineResponse>> GetMovementTimelineAsync(int days, CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        days = days < 1 ? 7 : days;

        const string sql =
            """
            SELECT d.date AS Date, COALESCE(COUNT(m.id), 0)::int AS Count
            FROM generate_series(
                CURRENT_DATE - @Days * INTERVAL '1 day',
                CURRENT_DATE,
                '1 day'
            ) AS d(date)
            LEFT JOIN inventory.movements m ON DATE(m.date) = d.date
            GROUP BY d.date
            ORDER BY d.date
            """;

        var rows = (await connection.QueryAsync<MovementTimelineResponse>(sql, new { Days = days })).ToList();
        return rows;
    }

    public async Task<IReadOnlyCollection<SupplierDistributionResponse>> GetSupplierDistributionAsync(CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT
                COALESCE(provider, 'Unknown') AS SupplierName,
                COUNT(*)::int AS ComponentCount
            FROM catalog.components
            WHERE provider IS NOT NULL AND provider != ''
            GROUP BY provider
            ORDER BY ComponentCount DESC
            """;

        var rows = (await connection.QueryAsync<SupplierDistributionResponse>(sql)).ToList();
        return rows;
    }

    public async Task<IReadOnlyCollection<CategoryDistributionResponse>> GetCategoryDistributionAsync(CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT
                COALESCE(c.name, 'Uncategorized') AS CategoryName,
                COUNT(p.id)::int AS ComponentCount,
                COALESCE(SUM(COALESCE(p.unit_price, 0)), 0) AS TotalValue
            FROM catalog.components p
            LEFT JOIN catalog.categories c ON c.id = p.category_id
            GROUP BY c.name
            ORDER BY ComponentCount DESC
            """;

        var rows = (await connection.QueryAsync<CategoryDistributionResponse>(sql)).ToList();
        return rows;
    }
}
