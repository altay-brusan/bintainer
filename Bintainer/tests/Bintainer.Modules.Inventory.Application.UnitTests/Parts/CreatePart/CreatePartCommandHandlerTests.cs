using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Application.Parts.CreatePart;
using Bintainer.Modules.Inventory.Domain.Categories;
using Bintainer.Modules.Inventory.Domain.Footprints;
using Bintainer.Modules.Inventory.Domain.Parts;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Parts.CreatePart;

public class CreatePartCommandHandlerTests
{
    private readonly IPartRepository _partRepository = Substitute.For<IPartRepository>();
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IFootprintRepository _footprintRepository = Substitute.For<IFootprintRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreatePartCommandHandler _handler;

    public CreatePartCommandHandlerTests()
    {
        _handler = new CreatePartCommandHandler(
            _partRepository, _categoryRepository, _footprintRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithId()
    {
        var command = new CreatePartCommand(
            "PN-001", "MPN-001", "Resistor", null, null, null, null, null, null, null, null, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_WithCategoryId_ValidatesCategory()
    {
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Test");
        _categoryRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns(category);

        var command = new CreatePartCommand(
            "PN", "MPN", "Desc", null, null, null, null, null, categoryId, null, null, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ReturnsFailure()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns((Category?)null);

        var command = new CreatePartCommand(
            "PN", "MPN", "Desc", null, null, null, null, null, categoryId, null, null, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Categories.NotFound");
    }

    [Fact]
    public async Task Handle_FootprintNotFound_ReturnsFailure()
    {
        var footprintId = Guid.NewGuid();
        _footprintRepository.GetByIdAsync(footprintId, Arg.Any<CancellationToken>())
            .Returns((Footprint?)null);

        var command = new CreatePartCommand(
            "PN", "MPN", "Desc", null, null, null, null, null, null, footprintId, null, null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Footprints.NotFound");
    }

    [Fact]
    public async Task Handle_Success_CallsInsertAndSave()
    {
        var command = new CreatePartCommand(
            "PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null);

        await _handler.Handle(command, CancellationToken.None);

        _partRepository.Received(1).Insert(Arg.Is<Part>(p => p.PartNumber == "PN"));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
