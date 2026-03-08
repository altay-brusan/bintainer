using Bintainer.Common.Application.ActivityLog;
using Bintainer.Common.Application.Authorization;
using Bintainer.Modules.Catalog.Application.Abstractions.Data;
using Bintainer.Modules.Catalog.Application.Categories.CreateCategory;
using Bintainer.Modules.Catalog.Domain.Categories;

namespace Bintainer.Modules.Catalog.Application.UnitTests.Categories.CreateCategory;

public class CreateCategoryCommandHandlerTests
{
    private readonly ICategoryRepository _categoryRepository = Substitute.For<ICategoryRepository>();
    private readonly IActivityLogger _activityLogger = Substitute.For<IActivityLogger>();
    private readonly ICurrentUserService _currentUserService = Substitute.For<ICurrentUserService>();
    private readonly IUnitOfWork _unitOfWork = Substitute.For<IUnitOfWork>();
    private readonly CreateCategoryCommandHandler _handler;

    public CreateCategoryCommandHandlerTests()
    {
        _handler = new CreateCategoryCommandHandler(_categoryRepository, _activityLogger, _currentUserService, _unitOfWork);
    }

    [Fact]
    public async Task Handle_RootCategory_ReturnsSuccess()
    {
        var command = new CreateCategoryCommand("Electronics", null);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Handle_ChildCategory_ValidatesParent()
    {
        var parentId = Guid.NewGuid();
        var parent = Category.Create("Parent");
        _categoryRepository.GetByIdAsync(parentId, Arg.Any<CancellationToken>()).Returns(parent);

        var command = new CreateCategoryCommand("Child", parentId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task Handle_ParentNotFound_ReturnsFailure()
    {
        var parentId = Guid.NewGuid();
        _categoryRepository.GetByIdAsync(parentId, Arg.Any<CancellationToken>()).Returns((Category?)null);

        var command = new CreateCategoryCommand("Child", parentId);
        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error.Code.Should().Be("Categories.NotFound");
    }

    [Fact]
    public async Task Handle_Success_CallsInsertAndSave()
    {
        var command = new CreateCategoryCommand("Electronics", null);

        await _handler.Handle(command, CancellationToken.None);

        _categoryRepository.Received(1).Insert(Arg.Is<Category>(c => c.Name == "Electronics"));
        await _unitOfWork.Received(1).SaveChangesAsync(Arg.Any<CancellationToken>());
    }
}
