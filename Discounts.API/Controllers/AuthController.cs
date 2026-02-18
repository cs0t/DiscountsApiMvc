using System.Security.Claims;
using Discounts.API.Requests.AuthRequests;
using Discounts.Application.Commands;
using Discounts.Application.Interfaces.AuthContracts;
using Discounts.Application.Models;
using MapsterMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Discounts.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IMapper _mapper;
    
    public AuthController(IAuthService authService, IMapper mapper)
    {
        _authService = authService;
        _mapper = mapper;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Login(
        [FromBody] LoginRequest request, 
        CancellationToken ct = default)
    {
        var loginCommand = _mapper.Map<LoginCommand>(request);
        var response = await _authService.LoginAsync(loginCommand, ct);
        return Ok(response);
    }
    
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<LoginResponse>> Register(
        [FromBody] RegisterRequest request, 
        CancellationToken ct = default)
    {
        var registerCommand = _mapper.Map<RegisterCommand>(request);
        var response = await _authService.RegisterAsync(registerCommand, ct);
        return Ok(response);
    }
    
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<UserDto>> GetCurrentUser(CancellationToken ct = default)
    {
        var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var response =  await _authService.GetCurrentUserAsync(userId, ct);
        return Ok(response);
    }
}