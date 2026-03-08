using Bintainer.Modules.Catalog.Application.BomImports.GetBomHistory;

namespace Bintainer.Modules.Catalog.Application.BomImports;

public interface IBomImportReadService
{
    Task<BomHistoryPagedResponse> GetPagedAsync(int page, int pageSize, CancellationToken ct);
}
