using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Bins;

public static class BinErrors
{
    public static Error NotFound(Guid binId) =>
        Error.NotFound("Bins.NotFound", $"The bin with Id '{binId}' was not found.");

    public static Error HasComponents(Guid binId) =>
        Error.Conflict("Bins.HasComponents", $"The bin with Id '{binId}' contains components and cannot be deactivated.");

    public static readonly Error Inactive =
        Error.Problem("Bins.Inactive", "The bin is inactive and cannot accept components.");
}
