using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Footprints.UpdateFootprint;

public sealed record UpdateFootprintCommand(Guid FootprintId, string Name) : ICommand;
