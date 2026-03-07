using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Application.Parts.DeletePart;
using Bintainer.Modules.Inventory.Domain.Parts;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Parts.DeletePart;

public class DeletePartCommandHandlerTests
{
    private readonly IPartRepository _partRepository = Substitute.For<IPartRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeletePartCommandHandler _handler;

    public DeletePartCommandHandlerTests()
    {
        _handler = new DeletePartCommandHandler(_partRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_PartFound_ReturnsSuccess()
    {
        var partId = Guid.NewGuid();
        var part = Part.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null);
        _partRepository.GetByIdAsync(partId, Arg.Any<CancellationToken>()).Returns(part);

        var command = new DeletePartCommand(partId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_PartFound_CallsRemove()
    {
        var partId = Guid.NewGuid();
        var part = Part.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null);
        _partRepository.GetByIdAsync(partId, Arg.Any<CancellationToken>()).Returns(part);

        var command = new DeletePartCommand(partId);
        await _handler.Handle(command, CancellationToken.None);

        _partRepository.Received(1).Remove(part);
    }

    [Fact]
    public async Task Handle_PartFound_RaisesDomainEvent()
    {
        var partId = Guid.NewGuid();
        var part = Part.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null);
        _partRepository.GetByIdAsync(partId, Arg.Any<CancellationToken>()).Returns(part);

        var command = new DeletePartCommand(partId);
        await _handler.Handle(command, CancellationToken.None);

        part.DomainEvents.Should().Contain(e => e is PartDeletedDomainEvent);
    }

    [Fact]
    public async Task Handle_PartNotFound_ReturnsFailure()
    {
        var partId = Guid.NewGuid();
        _partRepository.GetByIdAsync(partId, Arg.Any<CancellationToken>()).Returns((Part?)null);

        var command = new DeletePartCommand(partId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Parts.NotFound");
    }
}
