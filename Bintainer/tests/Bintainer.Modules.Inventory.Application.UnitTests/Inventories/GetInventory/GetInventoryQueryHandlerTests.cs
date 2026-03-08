using Bintainer.Modules.Inventory.Application.Inventories;
using Bintainer.Modules.Inventory.Application.Inventories.GetInventory;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Inventories.GetInventory;

public class GetInventoryQueryHandlerTests
{
    private readonly IInventoryReadService _inventoryReadService = Substitute.For<IInventoryReadService>();

    [Fact]
    public async Task Handle_InventoryExists_ReturnsResponse()
    {
        var inventoryId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var response = new InventoryResponse(inventoryId, "Test Inventory", userId);
        _inventoryReadService.GetByIdAsync(inventoryId, Arg.Any<CancellationToken>()).Returns(response);

        var handler = new GetInventoryQueryHandler(_inventoryReadService);
        var query = new GetInventoryQuery(inventoryId);
        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Test Inventory");
    }

    [Fact]
    public async Task Handle_InventoryNotFound_ReturnsFailure()
    {
        _inventoryReadService.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((InventoryResponse?)null);

        var handler = new GetInventoryQueryHandler(_inventoryReadService);
        var query = new GetInventoryQuery(Guid.NewGuid());
        var result = await handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Inventories.NotFound");
    }
}
