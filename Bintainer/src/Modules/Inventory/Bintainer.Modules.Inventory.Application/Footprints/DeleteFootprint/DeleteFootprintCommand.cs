using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Footprints.DeleteFootprint;

public sealed record DeleteFootprintCommand(Guid FootprintId) : ICommand;
