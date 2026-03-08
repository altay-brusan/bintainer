using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Bins.DeactivateBin;

public sealed record DeactivateBinCommand(Guid BinId) : ICommand;
