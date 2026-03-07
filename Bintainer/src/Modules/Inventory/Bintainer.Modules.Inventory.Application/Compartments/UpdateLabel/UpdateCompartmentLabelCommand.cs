using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Compartments.UpdateLabel;

public sealed record UpdateCompartmentLabelCommand(Guid CompartmentId, string Label) : ICommand;
