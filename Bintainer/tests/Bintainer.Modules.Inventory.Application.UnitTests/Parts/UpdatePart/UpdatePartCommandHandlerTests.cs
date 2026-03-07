using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Application.Parts.UpdatePart;
using Bintainer.Modules.Inventory.Domain.Parts;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Parts.UpdatePart;

public class UpdatePartCommandHandlerTests
{
    private readonly IPartRepository _partRepository = Substitute.For<IPartRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdatePartCommandHandler _handler;

    public UpdatePartCommandHandlerTests()
    {
        _handler = new UpdatePartCommandHandler(_partRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_PartFound_ReturnsSuccess()
    {
        var partId = Guid.NewGuid();
        var part = Part.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null);
        _partRepository.GetByIdAsync(partId, Arg.Any<CancellationToken>()).Returns(part);

        var command = new UpdatePartCommand(
            partId, "PN-2", "MPN-2", "Desc 2", null, null, null, null, null, null, null, null, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_PartNotFound_ReturnsFailure()
    {
        var partId = Guid.NewGuid();
        _partRepository.GetByIdAsync(partId, Arg.Any<CancellationToken>()).Returns((Part?)null);

        var command = new UpdatePartCommand(
            partId, "PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Parts.NotFound");
    }

    [Fact]
    public async Task Handle_Success_CallsSaveChanges()
    {
        var partId = Guid.NewGuid();
        var part = Part.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null);
        _partRepository.GetByIdAsync(partId, Arg.Any<CancellationToken>()).Returns(part);

        var command = new UpdatePartCommand(
            partId, "PN-2", "MPN-2", "Desc 2", null, null, null, null, null, null, null, null, null);

        await _handler.Handle(command, CancellationToken.None);

        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
