using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Application.Categories.UpdateCategory;
using Bintainer.Modules.Catalog.Domain.Categories;

namespace Bintainer.Modules.Catalog.Application.UnitTests.Categories.UpdateCategory;

public class UpdateCategoryCommandHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly UpdateCategoryCommandHandler _handler;

    public UpdateCategoryCommandHandlerTests()
    {
        _handler = new UpdateCategoryCommandHandler(_categoryRepository, _unitOfWork);
    }

    [Fact]
    public async Task Handle_ValidUpdate_ReturnsSuccess()
    {
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Old");
        _categoryRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns(category);

        var command = new UpdateCategoryCommand(categoryId, "New", null);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ReturnsFailure()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns((Category?)null);

        var command = new UpdateCategoryCommand(categoryId, "New", null);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Categories.NotFound");
    }

    [Fact]
    public async Task Handle_SelfReference_ReturnsCircularError()
    {
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Self");
        _categoryRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns(category);

        var command = new UpdateCategoryCommand(categoryId, "Self", categoryId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Categories.CircularReference");
    }
}
