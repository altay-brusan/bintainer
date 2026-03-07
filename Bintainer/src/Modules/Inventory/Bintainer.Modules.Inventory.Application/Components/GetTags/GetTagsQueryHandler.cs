using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.Components.GetTags;

internal sealed class GetTagsQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetTagsQuery, IReadOnlyCollection<string>>
{
    public async Task<Result<IReadOnlyCollection<string>>> Handle(GetTagsQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql = "SELECT DISTINCT tags FROM inventory.components WHERE tags IS NOT NULL AND tags != ''";

        var rows = await connection.QueryAsync<string>(sql);

        var tags = rows
            .SelectMany(t => t.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(t => t, StringComparer.OrdinalIgnoreCase)
            .ToList();

        return tags;
    }
}
