using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.Authorization;
using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Application.Inventories.CreateInventory;
using Bintainer.Modules.Inventory.Domain.Inventories;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Inventories.CreateInventory;

public class CreateInventoryCommandHandlerTests
{
    private readonly IInventoryRepository _inventoryRepository = Substitute.For<IInventoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly IActivityLogger _activityLogger = Substitute.For<IActivityLogger>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly CreateInventoryCommandHandler _handler;

    public CreateInventoryCommandHandlerTests()
    {
        _handler = new CreateInventoryCommandHandler(
            _inventoryRepository, _unitOfWork, _activityLogger, _currentUserService);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithId()
    {
        var command = new CreateInventoryCommand("My Inventory", Guid.NewGuid());

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_CallsInsertOnRepository()
    {
        var userId = Guid.NewGuid();
        var command = new CreateInventoryCommand("My Inventory", userId);

        await _handler.Handle(command, CancellationToken.None);

        _inventoryRepository.Received(1).Insert(Arg.Is<Domain.Inventories.Inventory>(i =>
            i.Name == "My Inventory" && i.UserId == userId));
    }

    [Fact]
    public async Task Handle_CallsSaveChanges()
    {
        var command = new CreateInventoryCommand("My Inventory", Guid.NewGuid());

        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
