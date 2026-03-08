using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Compartments;

public static class CompartmentErrors
{
    public static Error NotFound(Guid compartmentId) =>
        Error.NotFound("Compartments.NotFound", $"The compartment with Id '{compartmentId}' was not found.");

    public static Error HasComponent(Guid compartmentId) =>
        Error.Conflict("Compartments.HasComponent", $"The compartment with Id '{compartmentId}' contains a component and cannot be deactivated.");

    public static readonly Error Inactive =
        Error.Problem("Compartments.Inactive", "The compartment is inactive and cannot accept components.");
}
