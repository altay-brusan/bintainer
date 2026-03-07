using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.Footprints.GetFootprints;

internal sealed class GetFootprintsQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetFootprintsQuery, IReadOnlyCollection<FootprintResponse>>
{
    public async Task<Result<IReadOnlyCollection<FootprintResponse>>> Handle(GetFootprintsQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        const string sql =
            """
            SELECT id AS Id, name AS Name
            FROM inventory.footprints
            ORDER BY name
            """;

        var footprints = await connection.QueryAsync<FootprintResponse>(sql);

        return footprints.ToList();
    }
}
