using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.StorageUnits.DeleteStorageUnit;

public sealed record DeleteStorageUnitCommand(Guid StorageUnitId) : ICommand;
