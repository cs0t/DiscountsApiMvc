using Discounts.Application.Commands.Admin.Users;
using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Handlers.Admin.Users;

public class BlockUserHandler(IUserRepository userRepository) 
    : IRequestHandler<ManageUserBlockCommand>
{
    public async Task Handle(ManageUserBlockCommand command, CancellationToken ct = default)
    {
        var user = await userRepository.GetWithRolesAsync(command.Id, ct);
        
        if (user is null)
            throw new UserNotFoundException("User not found");
        
        if(user.RoleId == (int)RoleEnum.Administrator)
            throw new ForbiddenException("You cannot block another administrator !");
        
        user.RoleId = (int)RoleEnum.Blocked;
        await userRepository.SaveChangesAsync(ct);
    }
}