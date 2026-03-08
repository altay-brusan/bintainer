using Bintainer.Modules.Inventory.Application.StorageUnits;
using Bintainer.Modules.Inventory.Application.StorageUnits.GetStorageUnit;

namespace Bintainer.Modules.Inventory.Application.UnitTests.StorageUnits.GetStorageUnit;

public class GetStorageUnitQueryHandlerTests
{
    private readonly IStorageUnitReadService _storageUnitReadService = Substitute.For<IStorageUnitReadService>();

    [Fact]
    public void Handler_CanBeConstructed()
    {
        var handler = new GetStorageUnitQueryHandler(_storageUnitReadService);

        handler.Should().NotBeNull();
    }

    [Fact]
    public async Task Handle_StorageUnitExists_ReturnsResponse()
    {
        var storageUnitId = Guid.NewGuid();
        var response = new StorageUnitResponse(storageUnitId, "Test", 3, 2, 6, Guid.NewGuid(), []);
        _storageUnitReadService.GetByIdAsync(storageUnitId, Arg.Any<CancellationToken>()).Returns(response);

        var handler = new GetStorageUnitQueryHandler(_storageUnitReadService);
        var query = new GetStorageUnitQuery(storageUnitId);
        var result = await handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Name.Should().Be("Test");
    }

    [Fact]
    public async Task Handle_StorageUnitNotFound_ReturnsFailure()
    {
        _storageUnitReadService.GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>())
            .Returns((StorageUnitResponse?)null);

        var handler = new GetStorageUnitQueryHandler(_storageUnitReadService);
        var query = new GetStorageUnitQuery(Guid.NewGuid());
        var result = await handler.Handle(query, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
    }
}
