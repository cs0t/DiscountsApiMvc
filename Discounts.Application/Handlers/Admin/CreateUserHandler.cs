using Discounts.Application.Commands.Admin;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Handlers.Admin;

public class CreateUserHandler(IUserRepository userRepository, IRoleRepository roleRepository) 
    : IRequestHandler<ManageUserCreationCommand, int>
{
    public async Task<int> Handle(ManageUserCreationCommand command, CancellationToken ct = default)
    {
        if(command.Password != command.ConfirmPassword)
            throw new ApplicationException("Passwords do not match !");

        if (await userRepository
                .ExistsAsync(u =>
                    u.Email == command.Email || u.UserName == command.UserName, ct))
        {
            throw new ApplicationException("A user with the same email or username already exists !");
        }
        
        if (!await roleRepository.ExistsAsync(r=>r.Id == command.RoleId, ct))
        {
            throw new ApplicationException("Role not found !");
        }
        
        var user = new User
        {
            Email = command.Email,
            UserName = command.UserName,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(command.Password),
            RoleId = command.RoleId,
        };
        
        await userRepository.Add(user, ct);
        await userRepository.SaveChangesAsync(ct);
        return user.Id;
    }
}