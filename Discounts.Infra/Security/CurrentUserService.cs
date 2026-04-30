using System.Security.Claims;
using Discounts.Application.Interfaces.AuthContracts;
using Microsoft.AspNetCore.Http;

namespace Discounts.Infra.Security;

public class CurrentUserService(HttpContextAccessor contextAccessor) : ICurrentUserService
{
    public int UserId => 
        int.Parse(contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
    
    public bool IsAuthenticated => contextAccessor.HttpContext?.User.Identity?.IsAuthenticated ?? false;

    public string? Role => contextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
}