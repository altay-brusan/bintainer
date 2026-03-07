using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.Categories.GetCategories;

internal sealed class GetCategoriesQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetCategoriesQuery, IReadOnlyCollection<CategoryResponse>>
{
    public async Task<Result<IReadOnlyCollection<CategoryResponse>>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT id AS Id, name AS Name, parent_id AS ParentId
            FROM inventory.categories
            ORDER BY name
            """;

        var rows = (await connection.QueryAsync<CategoryRow>(sql)).ToList();

        var lookup = rows.ToDictionary(r => r.Id);
        var roots = new List<CategoryResponse>();

        var responses = rows.ToDictionary(
            r => r.Id,
            r => new CategoryResponse(r.Id, r.Name, r.ParentId, []));

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
