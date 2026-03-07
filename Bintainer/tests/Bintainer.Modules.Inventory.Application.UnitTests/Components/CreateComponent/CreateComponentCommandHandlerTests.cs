using Bintainer.Modules.Inventory.Application.Abstractions.Data;
using Bintainer.Modules.Inventory.Application.Components.CreateComponent;
using Bintainer.Modules.Inventory.Domain.Categories;
using Bintainer.Modules.Inventory.Domain.Components;
using Bintainer.Modules.Inventory.Domain.Footprints;

namespace Bintainer.Modules.Inventory.Application.UnitTests.Components.CreateComponent;

public class CreateComponentCommandHandlerTests
{
    private readonly IComponentRepository _componentRepository = Substitute.For<IComponentRepository>();
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IFootprintRepository _footprintRepository = Substitute.For<IFootprintRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateComponentCommandHandler _handler;

    public CreateComponentCommandHandlerTests()
    {
        _handler = new CreateComponentCommandHandler(
            _componentRepository, _categoryRepository, _footprintRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsSuccessWithId()
    {
        var command = new CreateComponentCommand(
            "PN-001", "MPN-001", "Resistor", null, null, null, null, null, null, null, null, null, null, null, 0);

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

        var command = new CreateComponentCommand(
            "PN", "MPN", "Desc", null, null, null, null, null, categoryId, null, null, null, null, null, 0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ReturnsFailure()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
            .Returns((Category?)null);

        var command = new CreateComponentCommand(
            "PN", "MPN", "Desc", null, null, null, null, null, categoryId, null, null, null, null, null, 0);

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

        var command = new CreateComponentCommand(
            "PN", "MPN", "Desc", null, null, null, null, null, null, footprintId, null, null, null, null, 0);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Footprints.NotFound");
    }

    [Fact]
    public async Task Handle_Success_CallsInsertAndSave()
    {
        var command = new CreateComponentCommand(
            "PN", "MPN", "Desc", null, null, null, null, null, null, null, null, null, null, null, 0);

        await _handler.Handle(command, CancellationToken.None);

        _componentRepository.Received(1).Insert(Arg.Is<Component>(p => p.PartNumber == "PN"));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
