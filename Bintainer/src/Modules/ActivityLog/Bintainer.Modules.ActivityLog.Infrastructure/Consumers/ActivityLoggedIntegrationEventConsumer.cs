using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.EventBus;
using MassTransit;

namespace Bintainer.Modules.ActivityLog.Infrastructure.Consumers;

internal sealed class ActivityLoggedIntegrationEventConsumer(IActivityLogger activityLogger)
    : IConsumer<ActivityLoggedIntegrationEvent>
{
    public Task Consume(ConsumeContext<ActivityLoggedIntegrationEvent> context)
    {
        var msg = context.Message;

        return activityLogger.LogAsync(
            msg.UserId,
            msg.Action,
            msg.EntityType,
            msg.EntityId,
            msg.EntityName,
            msg.Message,
            details: msg.Details,
            ct: context.CancellationToken);
    }
}
