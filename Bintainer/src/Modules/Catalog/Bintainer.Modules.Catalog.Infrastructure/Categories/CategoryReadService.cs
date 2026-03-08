using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Modules.Catalog.Application.Categories;
using Bintainer.Modules.Catalog.Application.Categories.GetCategories;
using Dapper;

namespace Bintainer.Modules.Catalog.Infrastructure.Categories;

internal sealed class CategoryReadService(IDbConnectionFactory dbConnectionFactory) : ICategoryReadService
{
    public async Task<IReadOnlyCollection<CategoryResponse>> GetAllAsync(CancellationToken ct)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT id AS Id, name AS Name, parent_id AS ParentId
            FROM inventory.categories
            ORDER BY name
            """;

        var rows = (await connection.QueryAsync<CategoryRow>(sql)).ToList();

        var responses = rows.ToDictionary(
            r => r.Id,
            r => new CategoryResponse(r.Id, r.Name, r.ParentId, []));

        var roots = new List<CategoryResponse>();

        foreach (var response in responses.Values)
        {
            if (response.ParentId.HasValue && responses.TryGetValue(response.ParentId.Value, out var parent))
            {
                parent.Children.Add(response);
            }
            else
            {
                roots.Add(response);
            }
        }

        return roots;
    }

    private sealed record CategoryRow(Guid Id, string Name, Guid? ParentId);
}
