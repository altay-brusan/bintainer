using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Movements;

public static class MovementErrors
{
    public static Error InvalidAction(string action) =>
        Error.Problem("Movements.InvalidAction", $"The action '{action}' is not valid.");

    public static Error InsufficientQuantity() =>
        Error.Problem("Movements.InsufficientQuantity", "Insufficient quantity in the compartment.");
}
