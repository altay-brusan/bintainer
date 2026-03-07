using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Compartments.AssignComponent;

public sealed record AssignComponentToCompartmentCommand(Guid CompartmentId, Guid ComponentId, int Quantity) : ICommand;
