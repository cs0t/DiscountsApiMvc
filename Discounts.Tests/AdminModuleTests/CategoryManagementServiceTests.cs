using System.Linq.Expressions;
using Discounts.Application.Commands;
using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.CategoryExceptions;
using Discounts.Application.Interfaces.AdminModuleContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Services.AdminModuleServices;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Discounts.Tests.AdminModuleTests;


public class CategoryManagementServiceTests
{
    private readonly ICategoryManagementService _categoryManagementService;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    
    public CategoryManagementServiceTests()    
    {
        _categoryRepositoryMock = new Mock<ICategoryRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _categoryManagementService = new CategoryManagementService(_userRepositoryMock.Object, _categoryRepositoryMock.Object);
    }
    
    [Fact]
    public async Task CreateCategoryAsync_ShouldCreateCategory_WhenAdmin()
    {
        var adminId = 1;
        var command = new CreateCategoryCommand { Name = "Test Category", Description = "Test Description" };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = adminId, RoleId = (int)RoleEnum.Administrator });
        
        _categoryRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var result = await _categoryManagementService.CreateCategoryAsync(adminId, command);
        
        _categoryRepositoryMock.Verify(repo => repo.Add(It.Is<Category>(c => c.Name == command.Name && c.Description == command.Description), It.IsAny<CancellationToken>()), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateCategoryAsync_ShouldThrow_WhenNotAdmin()
    {
        var userId = 1;
        var command = new CreateCategoryCommand { Name = "Test Category", Description = "Test Description" };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Customer });
        
        _categoryRepositoryMock.Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        Func<Task> act = async () => await  _categoryManagementService.CreateCategoryAsync(userId, command);
        await act.Should().ThrowAsync<ForbiddenException>();
        _categoryRepositoryMock.Verify(repo => repo.Add(It.IsAny<Category>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ShouldThrow_WhenCategoryNotFound()
    {
        var userId = 1;
        var command = new UpdateCategoryCommand { Id = 1, NewName = "Updated Category", NewDescription = "Updated Description" };

        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Administrator });
        
        _categoryRepositoryMock.Setup(repo => repo.GetById(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category)null);
        
        Func<Task> act = async () => await  _categoryManagementService.UpdateCategoryAsync(userId, command);
        await act.Should().ThrowAsync<CategoryNotFoundException>().WithMessage("Category not found !");
        _categoryRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCategoryAsync_ShouldUpdateCategory_WhenAdmin()
    {
        var userId = 1;
        var command = new UpdateCategoryCommand
            { Id = 1, NewName = "Updated Category", NewDescription = "Updated Description" };

        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Administrator });

        var existingCategory = new Category { Id = command.Id, Name = "Old Category", Description = "Old Description" };
        _categoryRepositoryMock.Setup(repo => repo.GetById(command.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        await _categoryManagementService.UpdateCategoryAsync(userId, command);

        existingCategory.Name.Should().Be(command.NewName);
        existingCategory.Description.Should().Be(command.NewDescription);
        _categoryRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteCategoryAsync_ShouldThrow_WhenCategoryNotFound()
    {
        var userId = 1;
        var categoryId = 1;

        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Administrator });

        _categoryRepositoryMock.Setup(repo => repo.GetById(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Category)null);
       
        Func<Task> act = async () => await  _categoryManagementService.DeleteCategoryAsync(userId, categoryId);
        await act.Should().ThrowAsync<CategoryNotFoundException>().WithMessage("Category not found !");
        
        _categoryRepositoryMock.Verify(repo => repo.Delete(It.IsAny<Category>()), Times.Never);
    }
    
    [Fact]
    public async Task DeleteCategoryAsync_ShouldDeleteCategory_WhenAdmin()
    {
        var userId = 1;
        var categoryId = 1;

        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Administrator });

        var existingCategory = new Category { Id = categoryId, Name = "Test Category", Description = "Test Description" };
        _categoryRepositoryMock.Setup(repo => repo.GetById(categoryId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(existingCategory);

        await _categoryManagementService.DeleteCategoryAsync(userId, categoryId);

        _categoryRepositoryMock.Verify(repo => repo.Delete(existingCategory), Times.Once);
        _categoryRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}