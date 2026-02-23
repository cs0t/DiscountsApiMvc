using System.Linq.Expressions;
using System.Reflection;
using Discounts.Application.Commands;
using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.AdminModuleContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Services.AdminModuleServices;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Discounts.Tests.AdminModuleTests;

public class UserManagementServiceTests
{
    private readonly IUserManagementService _userManagementService;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IRoleRepository> _roleRepositoryMock;

    public UserManagementServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _roleRepositoryMock = new Mock<IRoleRepository>();
        _userManagementService = new UserManagementService(_userRepositoryMock.Object, _roleRepositoryMock.Object);
    }

    [Fact]
    public async Task CreateUserAsync_ShouldCreateUser_WhenAdmin()
    {
        var userId = 1;
        var command = new ManageUserCreationCommand
        {
            UserName = "Test",
            Email = "Test@gmail.com",
            Password = "123123",
            ConfirmPassword = "123123",
            RoleId = (int)RoleEnum.Customer
        };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Administrator });
        
        //new email and username
        _userRepositoryMock.Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        //role eixsts
        _roleRepositoryMock.Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var result = await _userManagementService.CreateUserAsync(userId, command);
        
        _userRepositoryMock.Verify(repo => repo
            .Add(It.Is<User>(u => u.Email == command.Email && u.UserName == command.UserName), 
                It.IsAny<CancellationToken>()), Times.Once);
        
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateUserAsync_ShouldThrow_WhenNotAdmin()
    {
        var userId = 1;
        var command = new ManageUserCreationCommand
        {
            UserName = "Test",
            Email = "Test@gmail.com",
            Password = "123123",
            ConfirmPassword = "123123",
            RoleId = (int)RoleEnum.Customer
        };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Customer });
        
        //new email and username
        _userRepositoryMock.Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        //role eixsts
        _roleRepositoryMock.Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var act  =  async() => await _userManagementService.CreateUserAsync(userId, command);
        
        await act.Should().ThrowAsync<ForbiddenException>();
        
        _userRepositoryMock.Verify(repo => repo
            .Add(It.IsAny<User>(), 
                It.IsAny<CancellationToken>()), Times.Never);
        
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task CreateUserAsync_ShouldThrow_WhenUsernameOrEmailAlreadyUsed()
    {
        var userId = 1;
        var command = new ManageUserCreationCommand
        {
            UserName = "Test",
            Email = "Test@gmail.com",
            Password = "123123",
            ConfirmPassword = "123123",
            RoleId = (int)RoleEnum.Customer
        };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Administrator });
        
        //existing email and username
        _userRepositoryMock.Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        //role eixsts
        _roleRepositoryMock.Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var act  =  async() => await _userManagementService.CreateUserAsync(userId, command);
        
        await act.Should().ThrowAsync<ApplicationException>();
        
        _userRepositoryMock.Verify(repo => repo
            .Add(It.IsAny<User>(), 
                It.IsAny<CancellationToken>()), Times.Never);
        
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task CreateUserAsync_ShouldThrow_WhenRoleNotFound()
    {
        var userId = 1;
        var command = new ManageUserCreationCommand
        {
            UserName = "Test",
            Email = "Test@gmail.com",
            Password = "123123",
            ConfirmPassword = "123123",
            RoleId = (int)RoleEnum.Customer
        };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Administrator });
        
        //existing email and username
        _userRepositoryMock.Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        //role doesnt eixst
        _roleRepositoryMock.Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var act  =  async() => await _userManagementService.CreateUserAsync(userId, command);
        
        await act.Should().ThrowAsync<ApplicationException>();
        
        _userRepositoryMock.Verify(repo => repo
            .Add(It.IsAny<User>(), 
                It.IsAny<CancellationToken>()), Times.Never);
        
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateUserAsync_ShouldUpdate_WhenAdmin()
    {
        var userId = 1;
        var command = new ManageUserUpdateCommand
        {
            UserId = 2,
            UserName = "Test",
            Email = "Test@gmail.com",
            Password = "123123",
            ConfirmPassword = "123123",
            RoleId = (int)RoleEnum.Customer
        };
        
        //admin exists
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Administrator });
        
        _userRepositoryMock.Setup(repo => repo.GetById(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = command.UserId, RoleId = (int)RoleEnum.Customer });
        
        
        _userRepositoryMock.Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        //role eixsts
        _roleRepositoryMock.Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var result = await _userManagementService.UpdateUserAsync(userId, command);
        
        // _userRepositoryMock.Verify(repo => repo
        //     .Add(It.Is<User>(u => u.Email == command.Email && u.UserName == command.UserName), 
        //         It.IsAny<CancellationToken>()), Times.Once);
        
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateUserAsync_ShouldThrow_WhenNotAdmin()
    {
        var userId = 1;
        var command = new ManageUserUpdateCommand
        {
            UserId = 2,
            UserName = "Test",
            Email = "Test@gmail.com",
            Password = "123123",
            ConfirmPassword = "123123",
            RoleId = (int)RoleEnum.Customer
        };
        
        //admin exists
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Customer });
        
        _userRepositoryMock.Setup(repo => repo.GetById(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = command.UserId, RoleId = (int)RoleEnum.Customer });
        
        
        _userRepositoryMock.Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        //role eixsts
        _roleRepositoryMock.Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var act = async() => await _userManagementService.UpdateUserAsync(userId, command);
        await act.Should().ThrowAsync<ForbiddenException>();
        
        // _userRepositoryMock.Verify(repo => repo
        //     .Add(It.Is<User>(u => u.Email == command.Email && u.UserName == command.UserName), 
        //         It.IsAny<CancellationToken>()), Times.Once);
        
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateUserAsync_ShouldThrow_WhenTryingToChangeOtherAdmin()
    {
        var userId = 1;
        var command = new ManageUserUpdateCommand
        {
            UserId = 2,
            UserName = "Test",
            Email = "Test@gmail.com",
            Password = "123123",
            ConfirmPassword = "123123",
            RoleId = (int)RoleEnum.Administrator
        };
        
        //admin exists
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Administrator });
        
        _userRepositoryMock.Setup(repo => repo.GetById(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = command.UserId, RoleId = (int)RoleEnum.Administrator });
        
        
        _userRepositoryMock.Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        //role eixsts
        _roleRepositoryMock.Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var act = async() => await _userManagementService.UpdateUserAsync(userId, command);
        await act.Should().ThrowAsync<ForbiddenException>();
        
        // _userRepositoryMock.Verify(repo => repo
        //     .Add(It.Is<User>(u => u.Email == command.Email && u.UserName == command.UserName), 
        //         It.IsAny<CancellationToken>()), Times.Once);
        
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateUserAsync_ShouldThrow_WhenUsingExistingEmail()
    {
        var userId = 1;
        var command = new ManageUserUpdateCommand
        {
            UserId = 2,
            UserName = "Test",
            Email = "Test@gmail.com",
            Password = "123123",
            ConfirmPassword = "123123",
            RoleId = (int)RoleEnum.Administrator
        };
        
        //admin exists
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Administrator });
        
        _userRepositoryMock.Setup(repo => repo.GetById(command.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = command.UserId, RoleId = (int)RoleEnum.Customer });
        
        
        _userRepositoryMock.Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<User, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        //role eixsts
        _roleRepositoryMock.Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<Role, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var act = async() => await _userManagementService.UpdateUserAsync(userId, command);
        await act.Should().ThrowAsync<ApplicationException>();
        
        // _userRepositoryMock.Verify(repo => repo
        //     .Add(It.Is<User>(u => u.Email == command.Email && u.UserName == command.UserName), 
        //         It.IsAny<CancellationToken>()), Times.Once);
        
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task BlockUserAsync_ShouldBlockUser_WhenAdmin()
    {
        var adminId = 1;
        var userId = 2;
        
        
        //admin exists
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = adminId, RoleId = (int)RoleEnum.Administrator });
        //user exists
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Customer });
        
        await _userManagementService.BlockUserAsync(adminId, userId, It.IsAny<CancellationToken>());
        
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task BlockUserAsync_ShouldThrow_WhenNotAdmin()
    {
        var adminId = 1;
        var userId = 2;
        
        
        //admin exists
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = adminId, RoleId = (int)RoleEnum.Customer });
        //user exists
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Customer });
        
        var act = async() => await _userManagementService.BlockUserAsync(adminId, userId, It.IsAny<CancellationToken>());
        await act.Should().ThrowAsync<ForbiddenException>();
        
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    
    [Fact]
    public async Task BlockUserAsync_ShouldThrow_WhenBlockingOtherAdmin()
    {
        var adminId = 1;
        var userId = 2;
        
        
        //admin exists
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = adminId, RoleId = (int)RoleEnum.Administrator });
        //user exists
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Administrator });
        
        var act = async() => await _userManagementService.BlockUserAsync(adminId, userId, It.IsAny<CancellationToken>());
        await act.Should().ThrowAsync<ForbiddenException>();
        
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task BlockUserAsync_ShouldThrow_WhenAdminNotFound()
    {
        var adminId = 1;
        var userId = 2;
        
        
        //admin doesnt exist
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);
        //user exists
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Customer });
        
        var act = async() => await _userManagementService.BlockUserAsync(adminId, userId, It.IsAny<CancellationToken>());
        await act.Should().ThrowAsync<UserNotFoundException>();
        
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task BlockUserAsync_ShouldThrow_WhenUserNotFound()
    {
        var adminId = 1;
        var userId = 2;
        
        
        //admin exists
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = adminId, RoleId = (int)RoleEnum.Administrator });
        //user doesnt exists
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);
        
        var act = async() => await _userManagementService.BlockUserAsync(adminId, userId, It.IsAny<CancellationToken>());
        await act.Should().ThrowAsync<UserNotFoundException>();
        
        _userRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task GetSettingsPagedForAdminAsync_ShouldThrow_WhenNotAdmin()
    {
        var userId = 1;
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Customer });
        
        var act = async() => await _userManagementService.GetSettingsPagedForAdminAsync(userId);
        await act.Should().ThrowAsync<ForbiddenException>();
    }
}