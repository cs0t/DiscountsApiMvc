using Discounts.Application.Commands;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Interfaces.JwtContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;

namespace Discounts.Application.Services.AuthServices;

public class AuthService : IAuthService
{
    private readonly IJwtService _jwtService;
    private readonly IUserRepository _userRepository;
    
    public AuthService(IJwtService jwtService, IUserRepository userRepository)
    {
        _jwtService = jwtService;
        _userRepository = userRepository;
    }
    
    public async Task<LoginResponse> LoginAsync(LoginCommand loginCommand, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByEmailAsync(loginCommand.Email, ct);
        
        if(user is null)
            throw new UserNotFoundException($"User with email {loginCommand.Email} not found !");
        
        if (!BCrypt.Net.BCrypt.Verify(loginCommand.Password, user.PasswordHash))
            throw new ApplicationException("Invalid password !");
        
        return _jwtService.GenerateToken(user);
    }

    public async Task<LoginResponse> RegisterAsync(RegisterCommand registerCommand, CancellationToken ct = default)
    {
        //check if email already exists
        var emailExists = await _userRepository.ExistsAsync(u=> u.Email == registerCommand.Email, ct);
        if (emailExists)
            throw new ApplicationException("Email already exists !");
        //check if username already exists
        var usernameExists = await _userRepository.ExistsAsync(u=> u.UserName == registerCommand.UserName, ct);
        if (usernameExists)
            throw new ApplicationException("Username already exists !");
        
        //create user
        var user = new User
        {
            UserName = registerCommand.UserName,
            Email = registerCommand.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerCommand.Password),
            RoleId = (int)RoleEnum.Customer
        };
        
        await _userRepository.Add(user, ct);
        await _userRepository.SaveChangesAsync(ct);
        return _jwtService.GenerateToken(user);
    }

    public async Task<UserDto> GetCurrentUserAsync(int userId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetById(userId, ct);
        
        if (user is null)
            throw new UserNotFoundException("User not found !");
        
        return new UserDto
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Role = user.Role.Name
        };
    }
}