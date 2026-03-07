using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Compartments;

public static class CompartmentErrors
{
    public static Error NotFound(Guid compartmentId) =>
        Error.NotFound("Compartments.NotFound", $"The compartment with Id '{compartmentId}' was not found.");
}
