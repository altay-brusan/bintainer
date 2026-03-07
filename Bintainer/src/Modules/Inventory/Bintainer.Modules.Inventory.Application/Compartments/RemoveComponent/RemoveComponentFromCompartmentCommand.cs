using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Compartments.RemoveComponent;

public sealed record RemoveComponentFromCompartmentCommand(Guid CompartmentId) : ICommand;
