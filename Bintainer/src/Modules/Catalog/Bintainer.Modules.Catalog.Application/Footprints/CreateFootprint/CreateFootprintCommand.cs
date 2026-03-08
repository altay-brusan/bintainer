using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Catalog.Application.Footprints.CreateFootprint;

public sealed record CreateFootprintCommand(string Name) : ICommand<Guid>;
