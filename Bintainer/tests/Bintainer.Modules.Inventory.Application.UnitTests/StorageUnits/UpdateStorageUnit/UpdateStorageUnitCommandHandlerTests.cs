using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Application.StorageUnits.UpdateStorageUnit;
using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Application.UnitTests.StorageUnits.UpdateStorageUnit;

public class UpdateStorageUnitCommandHandlerTests
{
    private readonly IStorageUnitRepository _storageUnitRepository = Substitute.For<IStorageUnitRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateStorageUnitCommandHandler _handler;

    public UpdateStorageUnitCommandHandlerTests()
    {
        _handler = new UpdateStorageUnitCommandHandler(_storageUnitRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_StorageUnitFound_ReturnsSuccess()
    {
        var storageUnitId = Guid.NewGuid();
        var storageUnit = StorageUnit.Create("Old Name", 1, 1, 1, Guid.NewGuid());
        _storageUnitRepository.GetByIdAsync(storageUnitId, Arg.Any<CancellationToken>())
            .Returns(storageUnit);

        var command = new UpdateStorageUnitCommand(storageUnitId, "New Name");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        storageUnit.Name.Should().Be("New Name");
    }

    [Fact]
    public async Task Handle_StorageUnitFound_CallsSaveChanges()
    {
        var storageUnitId = Guid.NewGuid();
        var storageUnit = StorageUnit.Create("Old Name", 1, 1, 1, Guid.NewGuid());
        _storageUnitRepository.GetByIdAsync(storageUnitId, Arg.Any<CancellationToken>())
            .Returns(storageUnit);

        var command = new UpdateStorageUnitCommand(storageUnitId, "New Name");
        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Handle_StorageUnitNotFound_ReturnsFailure()
    {
        var storageUnitId = Guid.NewGuid();
        _storageUnitRepository.GetByIdAsync(storageUnitId, Arg.Any<CancellationToken>())
            .Returns((StorageUnit?)null);

        var command = new UpdateStorageUnitCommand(storageUnitId, "New Name");
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("StorageUnits.NotFound");
    }
}
