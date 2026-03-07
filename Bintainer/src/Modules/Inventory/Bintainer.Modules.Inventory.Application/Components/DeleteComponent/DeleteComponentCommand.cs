using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Components.DeleteComponent;

public sealed record DeleteComponentCommand(Guid ComponentId) : ICommand;
