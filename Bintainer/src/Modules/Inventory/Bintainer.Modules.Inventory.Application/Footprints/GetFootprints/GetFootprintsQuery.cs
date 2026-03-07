using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Footprints.GetFootprints;

public sealed record GetFootprintsQuery() : IQuery<IReadOnlyCollection<FootprintResponse>>;

public sealed record FootprintResponse(Guid Id, string Name);
