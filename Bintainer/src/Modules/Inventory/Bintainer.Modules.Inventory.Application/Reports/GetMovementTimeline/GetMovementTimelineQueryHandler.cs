using System.Data.Common;
using Bintainer.Common.Application.Data;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Dapper;

namespace Bintainer.Modules.Inventory.Application.Reports.GetMovementTimeline;

internal sealed class GetMovementTimelineQueryHandler(
    IDbConnectionFactory dbConnectionFactory) : IQueryHandler<GetMovementTimelineQuery, IReadOnlyCollection<MovementTimelineResponse>>
{
    public async Task<Result<IReadOnlyCollection<MovementTimelineResponse>>> Handle(GetMovementTimelineQuery request, CancellationToken cancellationToken)
    {
        await using DbConnection connection = await dbConnectionFactory.OpenConnectionAsync();

        var days = request.Days < 1 ? 7 : request.Days;

        const string sql =
            """
            SELECT d.date AS Date, COALESCE(COUNT(m.id), 0) AS Count
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
}
