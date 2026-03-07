using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Application.Compartments.AssignPart;
using Bintainer.Modules.Inventory.Domain.Compartments;
using Bintainer.Modules.Inventory.Domain.Parts;
using Bintainer.Modules.Inventory.Domain.StorageUnits;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Compartments.AssignPart;

public class AssignPartToCompartmentCommandHandlerTests
{
    private readonly ICompartmentRepository _compartmentRepository = Substitute.For<ICompartmentRepository>();
    private readonly IPartRepository _partRepository = Substitute.For<IPartRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly AssignPartToCompartmentCommandHandler _handler;

    public AssignPartToCompartmentCommandHandlerTests()
    {
        _handler = new AssignPartToCompartmentCommandHandler(
            _compartmentRepository, _partRepository, _unitOfWork);
    }

    private static Compartment CreateCompartment()
    {
        var unit = StorageUnit.Create("Test", 1, 1, 1, Guid.NewGuid());
        return unit.Bins.First().Compartments.First();
    }

    [Fact]
    public async Task Handle_Valid_ReturnsSuccess()
    {
        var compartmentId = Guid.NewGuid();
        var partId = Guid.NewGuid();
        var compartment = CreateCompartment();
        var part = Part.Create("PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null);

        _compartmentRepository.GetByIdAsync(compartmentId, Arg.Any<CancellationToken>()).Returns(compartment);
        _partRepository.GetByIdAsync(partId, Arg.Any<CancellationToken>()).Returns(part);

        var command = new AssignPartToCompartmentCommand(compartmentId, partId, 5);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        compartment.PartId.Should().Be(partId);
        compartment.Quantity.Should().Be(5);
    }

    [Fact]
    public async Task Handle_CompartmentNotFound_ReturnsFailure()
    {
        var compartmentId = Guid.NewGuid();
        _compartmentRepository.GetByIdAsync(compartmentId, Arg.Any<CancellationToken>())
            .Returns((Compartment?)null);

        var command = new AssignPartToCompartmentCommand(compartmentId, Guid.NewGuid(), 5);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Compartments.NotFound");
    }

    [Fact]
    public async Task Handle_PartNotFound_ReturnsFailure()
    {
        var compartmentId = Guid.NewGuid();
        var partId = Guid.NewGuid();
        var compartment = CreateCompartment();
        _compartmentRepository.GetByIdAsync(compartmentId, Arg.Any<CancellationToken>()).Returns(compartment);
        _partRepository.GetByIdAsync(partId, Arg.Any<CancellationToken>()).Returns((Part?)null);

        var command = new AssignPartToCompartmentCommand(compartmentId, partId, 5);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Parts.NotFound");
    }
}
