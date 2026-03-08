using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Application.Messaging;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Domain.Inventories;

namespace Bintainer.Modules.Inventory.Application.Inventories.CreateInventory;

internal sealed class CreateInventoryCommandHandler(
    IInventoryRepository inventoryRepository,
    IUnitOfWork unitOfWork,
    IActivityLogger activityLogger,
    ICurrentUserService currentUserService) : ICommandHandler<CreateInventoryCommand, Guid>
{
    public async Task<Result<Guid>> Handle(CreateInventoryCommand request, CancellationToken cancellationToken)
    {
        var inventory = Domain.Inventories.Inventory.Create(request.Name, request.UserId);

        inventoryRepository.Insert(inventory);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        await activityLogger.LogAsync(
            currentUserService.UserId,
            "InventoryCreated",
            "Inventory",
            inventory.Id,
            request.Name,
            ct: cancellationToken);

        return inventory.Id;
    }
}
