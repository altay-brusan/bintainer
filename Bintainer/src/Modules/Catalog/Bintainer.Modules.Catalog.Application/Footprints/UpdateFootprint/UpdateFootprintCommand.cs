using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Catalog.Application.Footprints.UpdateFootprint;

public sealed record UpdateFootprintCommand(Guid FootprintId, string Name) : ICommand;
