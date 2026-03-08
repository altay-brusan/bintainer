using Bintainer.Common.Presentation.Results;
using Bintainer.Common.Presentation.Endpoints;
using Bintainer.Modules.ActivityLog.Application.GetActivityLog;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Bintainer.Modules.ActivityLog.Presentation;

internal sealed class GetActivityLog : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet("api/activity-log", async (
            string? action,
            string? entityType,
            Guid? entityId,
            int? page,
            int? pageSize,
            ISender sender) =>
        {
            var result = await sender.Send(new GetActivityLogQuery(
                action, entityType, entityId,
                page ?? 1, pageSize ?? 20));

            return result.Match(Results.Ok, ApiResults.Problem);
        })
        .RequireAuthorization()
        .WithTags("ActivityLog");
    }
}
