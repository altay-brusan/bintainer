using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.Authorization;
using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Application.Categories.DeleteCategory;
using Bintainer.Modules.Catalog.Domain.Categories;

namespace Bintainer.Modules.Catalog.Application.UnitTests.Categories.DeleteCategory;

public class DeleteCategoryCommandHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IActivityLogger _activityLogger = Substitute.For<IActivityLogger>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly DeleteCategoryCommandHandler _handler;

    public DeleteCategoryCommandHandlerTests()
    {
        _handler = new DeleteCategoryCommandHandler(_categoryRepository, _activityLogger, _currentUserService, _unitOfWork);
    }

    [Fact]
    public async Task Handle_EmptyCategory_ReturnsSuccess()
    {
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Test");
        _categoryRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns(category);
        _categoryRepository.HasChildrenAsync(categoryId, Arg.Any<CancellationToken>()).Returns(false);

        var command = new DeleteCategoryCommand(categoryId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_CategoryWithChildren_ReturnsFailure()
    {
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Test");
        _categoryRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns(category);
        _categoryRepository.HasChildrenAsync(categoryId, Arg.Any<CancellationToken>()).Returns(true);

        var command = new DeleteCategoryCommand(categoryId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Categories.HasChildren");
    }

    [Fact]
    public async Task Handle_CategoryNotFound_ReturnsFailure()
    {
        var categoryId = Guid.NewGuid();
        _categoryRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns((Category?)null);

        var command = new DeleteCategoryCommand(categoryId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Categories.NotFound");
    }

    [Fact]
    public async Task Handle_Success_CallsRemoveAndSave()
    {
        var categoryId = Guid.NewGuid();
        var category = Category.Create("Test");
        _categoryRepository.GetByIdAsync(categoryId, Arg.Any<CancellationToken>()).Returns(category);
        _categoryRepository.HasChildrenAsync(categoryId, Arg.Any<CancellationToken>()).Returns(false);

        var command = new DeleteCategoryCommand(categoryId);
        await _handler.Handle(command, CancellationToken.None);

        _categoryRepository.Received(1).Remove(category);
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
