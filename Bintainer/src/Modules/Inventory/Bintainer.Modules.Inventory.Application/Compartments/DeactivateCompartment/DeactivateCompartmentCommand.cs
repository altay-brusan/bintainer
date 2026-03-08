using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Compartments.DeactivateCompartment;

public sealed record DeactivateCompartmentCommand(Guid CompartmentId) : ICommand;
