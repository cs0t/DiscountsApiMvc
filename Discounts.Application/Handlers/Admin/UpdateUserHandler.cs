using Discounts.Application.Commands.Admin;
using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Handlers.Admin;

public class UpdateUserHandler(IUserRepository userRepository, IRoleRepository roleRepository) 
    : IRequestHandler<ManageUserUpdateCommand, int>
{
    public async Task<int> Handle(ManageUserUpdateCommand command, CancellationToken ct = default)
    {
        var user = await userRepository.GetById(command.UserId, ct);
        if (user is null)
            throw new UserNotFoundException("User not found");
        
        if(user.RoleId == (int)RoleEnum.Administrator)
            throw new ForbiddenException("You cannot update another administrator !");

        if (!string.IsNullOrEmpty(command.Password) || !string.IsNullOrEmpty(command.ConfirmPassword))
        {
            if (command.Password != command.ConfirmPassword)
                throw new ApplicationException("Passwords do not match !");
        }

        if (!string.IsNullOrEmpty(command.Email))
        {
            var emailExists = await userRepository.ExistsAsync(u => u.Email == command.Email && u.Id != command.UserId, ct);
            if (emailExists)
                throw new ApplicationException("A user with the same email already exists !");
        }

        if (!string.IsNullOrEmpty(command.UserName))
        {
            var usernameExists = await userRepository.ExistsAsync(u => u.UserName == command.UserName && u.Id != command.UserId, ct);
            if (usernameExists)
                throw new ApplicationException("A user with the same username already exists !");
        }

        if (command.RoleId.HasValue)
        {
            if (!await roleRepository.ExistsAsync(r => r.Id == command.RoleId.Value, ct))
            {
                throw new ApplicationException("Role not found !");
            }
            user.RoleId = command.RoleId.Value;
        }

        if (!string.IsNullOrEmpty(command.UserName))
            user.UserName = command.UserName!;

        if (!string.IsNullOrEmpty(command.Email))
            user.Email = command.Email!;

        if (!string.IsNullOrEmpty(command.Password))
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password);

        await userRepository.SaveChangesAsync(ct);
        return user.Id;
    }
}