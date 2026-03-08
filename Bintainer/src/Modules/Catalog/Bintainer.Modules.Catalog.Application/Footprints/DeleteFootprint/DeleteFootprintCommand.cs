using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Catalog.Application.Footprints.DeleteFootprint;

public sealed record DeleteFootprintCommand(Guid FootprintId) : ICommand;
