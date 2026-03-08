using Bintainer.Modules.Catalog.Application.Components.GetComponent;
using Bintainer.Modules.Catalog.Application.Components.GetComponents;
using Bintainer.Modules.Catalog.Application.Components.GetTags;
using Bintainer.Modules.Catalog.Application.Components.SearchComponents;

namespace Bintainer.Modules.Catalog.Application.Components;

public interface IComponentReadService
{
    Task<ComponentResponse?> GetByIdAsync(Guid id, CancellationToken ct);
    Task<IReadOnlyCollection<ComponentSummaryResponse>> GetByCategoryIdAsync(Guid? categoryId, CancellationToken ct);
    Task<SearchComponentsPagedResponse> SearchAsync(string? q, Guid? categoryId, string? provider, string? tag, Guid? footprintId, int page, int pageSize, CancellationToken ct);
    Task<IReadOnlyCollection<string>> GetTagsAsync(CancellationToken ct);
}
