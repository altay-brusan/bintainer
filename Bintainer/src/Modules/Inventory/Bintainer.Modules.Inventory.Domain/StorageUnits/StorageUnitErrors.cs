using Bintainer.Common.Domain;

namespace Bintainer.Modules.Inventory.Domain.StorageUnits;

public static class StorageUnitErrors
{
    public static Error NotFound(Guid storageUnitId) =>
        Error.NotFound("StorageUnits.NotFound", $"The storage unit with Id '{storageUnitId}' was not found.");
}
