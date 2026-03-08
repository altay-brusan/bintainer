using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.Authorization;
using Bintainer.Common.Domain;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Application.StorageUnits.CreateStorageUnit;
using Bintainer.Modules.Inventory.Domain.Inventories;
using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Application.UnitTests.StorageUnits.CreateStorageUnit;

public class CreateStorageUnitCommandHandlerTests
{
    private readonly IInventoryRepository _inventoryRepository = Substitute.For<IInventoryRepository>();
    private readonly IStorageUnitRepository _storageUnitRepository = Substitute.For<IStorageUnitRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IActivityLogger _activityLogger = Substitute.For<IActivityLogger>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly CreateStorageUnitCommandHandler _handler;

    public CreateStorageUnitCommandHandlerTests()
    {
        _handler = new CreateStorageUnitCommandHandler(
            _inventoryRepository, _storageUnitRepository, _unitOfWork, _activityLogger, _currentUserService);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithId()
    {
        var inventoryId = Guid.NewGuid();
        var inventory = Domain.Inventories.Inventory.Create("Test", Guid.NewGuid());
        _inventoryRepository.GetByIdAsync(inventoryId, Arg.Any<CancellationToken>())
            .Returns(inventory);

        var command = new CreateStorageUnitCommand("Shelf A", 2, 3, 4, inventoryId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_InventoryNotFound_ReturnsFailure()
    {
        var inventoryId = Guid.NewGuid();
        _inventoryRepository.GetByIdAsync(inventoryId, Arg.Any<CancellationToken>())
            .Returns((Domain.Inventories.Inventory?)null);

        var command = new CreateStorageUnitCommand("Shelf A", 2, 3, 4, inventoryId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Inventories.NotFound");
    }

    [Fact]
    public async Task Handle_Success_CallsInsert()
    {
        var inventoryId = Guid.NewGuid();
        var inventory = Domain.Inventories.Inventory.Create("Test", Guid.NewGuid());
        _inventoryRepository.GetByIdAsync(inventoryId, Arg.Any<CancellationToken>())
            .Returns(inventory);

        var command = new CreateStorageUnitCommand("Shelf A", 2, 3, 4, inventoryId);
        await _handler.Handle(command, CancellationToken.None);

        _storageUnitRepository.Received(1).Insert(Arg.Is<StorageUnit>(su =>
            su.Name == "Shelf A" && su.Columns == 2 && su.Rows == 3));
    }

    [Fact]
    public async Task Handle_Success_CallsSaveChanges()
    {
        var inventoryId = Guid.NewGuid();
        var inventory = Domain.Inventories.Inventory.Create("Test", Guid.NewGuid());
        _inventoryRepository.GetByIdAsync(inventoryId, Arg.Any<CancellationToken>())
            .Returns(inventory);

        var command = new CreateStorageUnitCommand("Shelf A", 2, 3, 4, inventoryId);
        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_InventoryNotFound_DoesNotCallInsert()
    {
        var inventoryId = Guid.NewGuid();
        _inventoryRepository.GetByIdAsync(inventoryId, Arg.Any<CancellationToken>())
            .Returns((Domain.Inventories.Inventory?)null);

        var command = new CreateStorageUnitCommand("Shelf A", 2, 3, 4, inventoryId);
        await _handler.Handle(command, CancellationToken.None);

        _storageUnitRepository.DidNotReceive().Insert(Arg.Any<StorageUnit>());
    }
}
