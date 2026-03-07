using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Components.AdjustComponentQuantity;

public sealed record AdjustComponentQuantityCommand(
    Guid ComponentId,
    Guid CompartmentId,
    string Action,
    int Quantity,
    string? Notes) : ICommand;
