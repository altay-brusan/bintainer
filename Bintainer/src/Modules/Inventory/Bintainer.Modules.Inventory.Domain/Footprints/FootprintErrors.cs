using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.Footprints;

public static class FootprintErrors
{
    public static Error NotFound(Guid footprintId) =>
        Error.NotFound("Footprints.NotFound", $"The footprint with Id '{footprintId}' was not found.");
}
