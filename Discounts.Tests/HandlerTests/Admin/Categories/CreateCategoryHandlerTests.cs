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
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly CreateCategoryHandler _handler;
    
    public CreateCategoryHandlerTests()
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _handler = new CreateCategoryHandler(_categoryRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateCategoryHandler_ShouldThrowException_WhenCategoryWithSameNameExists()
    {
        _categoryRepositoryMock
            .Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(),  It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var command = new CreateCategoryCommand ("Existing Category", null);
        await Assert.ThrowsAsync<ApplicationException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task CreateCategoryHandler_ShouldCreateCategory_WhenCategoryDoesNotExist()
    {
        
        _categoryRepositoryMock.Setup(repo => 
            repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        _categoryRepositoryMock
            .Setup(repo => repo.Add(It.IsAny<Category>(), It.IsAny<CancellationToken>()))
            .Callback(() => _categoryRepositoryMock)
            .Returns(Task.CompletedTask);
        
        _categoryRepositoryMock
            .Setup(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
            
        var command = new CreateCategoryCommand ("New Category", null);
        
        
    }
}