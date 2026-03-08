using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Modules.Catalog.Application.Footprints;
using Bintainer.Modules.Catalog.Application.Footprints.GetFootprints;
using Dapper;

namespace Bintainer.Modules.Catalog.Infrastructure.Footprints;

internal sealed class FootprintReadService(IDbConnectionFactory dbConnectionFactory) : IFootprintReadService
{
    public async Task<IReadOnlyCollection<FootprintResponse>> GetAllAsync(CancellationToken ct)
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
