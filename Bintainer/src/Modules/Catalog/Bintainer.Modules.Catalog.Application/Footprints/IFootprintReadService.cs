using Bintainer.Modules.Catalog.Application.Footprints.GetFootprints;

namespace Bintainer.Modules.Catalog.Application.Footprints;

public interface IFootprintReadService
{
    Task<IReadOnlyCollection<FootprintResponse>> GetAllAsync(CancellationToken ct);
}
