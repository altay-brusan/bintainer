using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Reports.GetMovementTimeline;

public sealed record GetMovementTimelineQuery(int Days) : IQuery<IReadOnlyCollection<MovementTimelineResponse>>;

public sealed record MovementTimelineResponse(
    DateTime Date,
    int Count);
