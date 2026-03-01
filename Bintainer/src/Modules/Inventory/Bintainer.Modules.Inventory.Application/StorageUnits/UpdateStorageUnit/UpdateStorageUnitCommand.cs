using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.StorageUnits.UpdateStorageUnit;

public sealed record UpdateStorageUnitCommand(Guid StorageUnitId, string Name) : ICommand;
