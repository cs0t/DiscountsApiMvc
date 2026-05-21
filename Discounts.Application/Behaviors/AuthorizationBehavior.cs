using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using MediatR;

namespace Discounts.Application.Behaviors;

public class AuthorizationBehavior<TRequest,TResponse>(ICurrentUserService currUserService, IUserRepository userRepository) 
    : IPipelineBehavior<TRequest,TResponse> where TRequest:IRequireRole
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct = default)
    {
        if (!currUserService.IsAuthenticated)
            throw new UnauthorizedException("User is not authorized !");
        
        var id = currUserService.UserId;
        
        var existingUser = 
            await userRepository.GetWithRolesAsync(id, ct) 
            ?? throw new UserNotFoundException($"User with id {id} not found !");

        if (existingUser.RoleId != (int)request.RoleRequired)
            throw new ForbiddenException("User does not have permission for this action !");
        
        return await next(ct);
    }
} 