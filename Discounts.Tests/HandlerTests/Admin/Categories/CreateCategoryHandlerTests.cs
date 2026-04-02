using System.Linq.Expressions;
using Discounts.Application.Commands.Admin.Categories;
using Discounts.Application.Handlers.Admin.Categories;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using Moq;

namespace Discounts.Tests.HandlerTests.Admin.Categories;

public class CreateCategoryHandlerTests
{
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock = new();

    [Fact]
    public async Task CreateCategoryHandler_ShouldThrowException_WhenCategoryWithSameNameExists()
    {
        _categoryRepositoryMock
            .Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(),  It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var handler = new CreateCategoryHandler(_categoryRepositoryMock.Object);
        var command = new CreateCategoryCommand ("Existing Category", null);
    }
}