using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Components.MoveComponent;

public sealed record MoveComponentCommand(
    Guid ComponentId,
    Guid SourceCompartmentId,
    Guid DestinationCompartmentId,
    int Quantity) : ICommand;
