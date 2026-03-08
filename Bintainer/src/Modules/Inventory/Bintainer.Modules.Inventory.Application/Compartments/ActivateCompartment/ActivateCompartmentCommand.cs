using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Compartments.ActivateCompartment;

public sealed record ActivateCompartmentCommand(Guid CompartmentId) : ICommand;
