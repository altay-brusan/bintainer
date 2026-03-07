using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Footprints.CreateFootprint;

public sealed record CreateFootprintCommand(string Name) : ICommand<Guid>;
