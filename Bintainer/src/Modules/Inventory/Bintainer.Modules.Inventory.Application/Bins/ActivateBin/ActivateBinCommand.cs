using Bintainer.Common.Application.Messaging;

namespace Bintainer.Modules.Inventory.Application.Bins.ActivateBin;

public sealed record ActivateBinCommand(Guid BinId) : ICommand;
