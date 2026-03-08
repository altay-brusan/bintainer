using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Reports.Application.GetMovementTimeline;

public sealed record GetMovementTimelineQuery(int Days) : IQuery<IReadOnlyCollection<MovementTimelineResponse>>;

public sealed record MovementTimelineResponse(
    DateTime Date,
    int Count);
