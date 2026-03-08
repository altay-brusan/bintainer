using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Catalog.Application.Components.DeleteComponent;

public sealed record DeleteComponentCommand(Guid ComponentId) : ICommand;
