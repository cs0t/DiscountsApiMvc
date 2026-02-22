using System.Linq.Expressions;
using Discounts.Application.Commands;
using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.SystemSettingsExceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.AdminModuleContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;
using FluentAssertions;
using Moq;

namespace Discounts.Tests.AdminModuleTests;

public class SystemSettingsManagementServiceTests
{
    private readonly ISystemSettingsManagementService _systemSettingsManagementService;
    private readonly Mock<ISystemSettingsRepository> _systemSettingsRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    
    public SystemSettingsManagementServiceTests()
    {
        _systemSettingsRepositoryMock = new Mock<ISystemSettingsRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _systemSettingsManagementService = new Application.Services.AdminModuleServices.SystemSettingsManagementService(_userRepositoryMock.Object, _systemSettingsRepositoryMock.Object);
    }
    
    [Fact]
    public async Task CreateSystemSettingAsync_ShouldCreateSetting_WhenAdminIsValid()
    {
        var adminId = 1;
        var command = new CreateSystemSettingCommand
        {
            Key = "TestKey",
            SettingValue = "TestValue"
        };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = adminId, RoleId = (int)Domain.Constants.RoleEnum.Administrator });
        
        _systemSettingsRepositoryMock
            .Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<SystemSettings, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var result = await _systemSettingsManagementService.CreateSystemSettingAsync(adminId, command);
        
        _systemSettingsRepositoryMock.Verify(repo => repo
            .Add(It.Is<SystemSettings>(s => s.Key == command.Key && s.SettingValue == command.SettingValue), 
                It.IsAny<CancellationToken>()), Times.Once);
        _systemSettingsRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task CreateSystemSettingAsync_ShouldThrow_WhenNotAdmin()
    {
        var userId = 1;
        var command = new CreateSystemSettingCommand
        {
            Key = "TestKey",
            SettingValue = "TestValue"
        };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Customer });
        
        _systemSettingsRepositoryMock
            .Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<SystemSettings, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);
        
        var act = async() => await _systemSettingsManagementService.CreateSystemSettingAsync(userId, command);

        await act.Should().ThrowAsync<ForbiddenException>();
        
        _systemSettingsRepositoryMock.Verify(repo => repo.Add(It.IsAny<SystemSettings>(), It.IsAny<CancellationToken>()), Times.Never);
        _systemSettingsRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task CreateSystemSettingAsync_ShouldThrow_WhenKeyAlreadyExists()
    {
        var adminId = 1;
        var command = new CreateSystemSettingCommand
        {
            Key = "TestKey",
            SettingValue = "TestValue"
        };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = adminId, RoleId = (int)RoleEnum.Administrator });
        
        _systemSettingsRepositoryMock
            .Setup(repo => 
                repo.ExistsAsync(It.IsAny<Expression<Func<SystemSettings, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);
        
        var act = async() => await _systemSettingsManagementService.CreateSystemSettingAsync(adminId, command);

        await act.Should().ThrowAsync<ApplicationException>();
        
        _systemSettingsRepositoryMock.Verify(repo => repo.Add(It.IsAny<SystemSettings>(), It.IsAny<CancellationToken>()), Times.Never);
        _systemSettingsRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateSystemSettingAsync_ShouldThrow_WhenUserNotFound()
    {
        var adminId = 1;
        var command = new CreateSystemSettingCommand
        {
            Key = "TestKey",
            SettingValue = "TestValue"
        };

        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);

        _systemSettingsRepositoryMock
            .Setup(repo =>
                repo.ExistsAsync(It.IsAny<Expression<Func<SystemSettings, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(false);

        var act = async () => await _systemSettingsManagementService.CreateSystemSettingAsync(adminId, command);

        await act.Should().ThrowAsync<UserNotFoundException>();

        _systemSettingsRepositoryMock.Verify(
            repo => repo.Add(It.IsAny<SystemSettings>(), It.IsAny<CancellationToken>()), Times.Never);
        _systemSettingsRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateSystemSettingAsync_ShouldUpdateSetting_WhenAdminIsValid()
    {
        var adminId = 1;
        var updateCommand = new UpdateSystemSettingCommand
        {
            Id = 1,
            NewSettingValue = "UpdatedValue"
        };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(adminId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = adminId, RoleId = (int)RoleEnum.Administrator });
        
        _systemSettingsRepositoryMock.Setup(repo => repo.GetById(updateCommand.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SystemSettings{Id = updateCommand.Id, SettingValue = "OldValue"});
        
        //var existingSetting = await _systemSettingsRepositoryMock.Object.GetById(updateCommand.Id, It.IsAny<CancellationToken>());
        
        await _systemSettingsManagementService.UpdateSystemSettingAsync(adminId, updateCommand, It.IsAny<CancellationToken>());
        
        _systemSettingsRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
    
    [Fact]
    public async Task UpdateSystemSettingAsync_ShouldThrow_WhenNotAdmin()
    {
        var userId = 1;
        var updateCommand = new UpdateSystemSettingCommand
        {
            Id = 1,
            NewSettingValue = "UpdatedValue"
        };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Customer });
        
        _systemSettingsRepositoryMock.Setup(repo => repo.GetById(updateCommand.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SystemSettings{Id = updateCommand.Id, SettingValue = "OldValue"});
        
        //var existingSetting = await _systemSettingsRepositoryMock.Object.GetById(updateCommand.Id, It.IsAny<CancellationToken>());
        
        var act = async() => await _systemSettingsManagementService
            .UpdateSystemSettingAsync(userId, updateCommand, It.IsAny<CancellationToken>());
        
        await act.Should().ThrowAsync<ForbiddenException>();
        
        _systemSettingsRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateSystemSettingAsync_ShouldThrow_WhenSettingNotFound()
    {
        var userId = 1;
        var updateCommand = new UpdateSystemSettingCommand
        {
            Id = 1,
            NewSettingValue = "UpdatedValue"
        };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User { Id = userId, RoleId = (int)RoleEnum.Administrator });
        
        _systemSettingsRepositoryMock.Setup(repo => repo.GetById(updateCommand.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((SystemSettings)null);
        
        //var existingSetting = await _systemSettingsRepositoryMock.Object.GetById(updateCommand.Id, It.IsAny<CancellationToken>());
        
        var act = async() => await _systemSettingsManagementService
            .UpdateSystemSettingAsync(userId, updateCommand, It.IsAny<CancellationToken>());
        
        await act.Should().ThrowAsync<SettingNotFoundException>();
        
        _systemSettingsRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    [Fact]
    public async Task UpdateSystemSettingAsync_ShouldThrow_WhenUserNotFound()
    {
        var userId = 1;
        var updateCommand = new UpdateSystemSettingCommand
        {
            Id = 1,
            NewSettingValue = "UpdatedValue"
        };
        
        _userRepositoryMock.Setup(repo => repo.GetWithRolesAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User)null);
        
        _systemSettingsRepositoryMock.Setup(repo => repo.GetById(updateCommand.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new SystemSettings{Id = updateCommand.Id, SettingValue = "OldValue"});
        
        //var existingSetting = await _systemSettingsRepositoryMock.Object.GetById(updateCommand.Id, It.IsAny<CancellationToken>());
        
        var act = async() => await _systemSettingsManagementService
            .UpdateSystemSettingAsync(userId, updateCommand, It.IsAny<CancellationToken>());
        
        await act.Should().ThrowAsync<UserNotFoundException>();
        
        _systemSettingsRepositoryMock.Verify(repo => repo.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
}