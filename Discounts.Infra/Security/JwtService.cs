using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Discounts.Application.Interfaces.JwtContracts;
using Discounts.Application.Models;
using Discounts.Domain.Entities;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Discounts.Infra.Security;

public class JwtService : IJwtService
{
    
    private readonly JwtSettings _jwtSettings;

    public JwtService(IOptions<JwtSettings> jwtSettings)
    {
        _jwtSettings = jwtSettings.Value;
    }
    
    public LoginResponse GenerateToken(User user)
    {
        var claims = new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.Name)
        };
        
        //key, signingcreds and expiration
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Secret));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);
        
        //creating jwt token
        var token = new JwtSecurityToken(issuer:_jwtSettings.Issuer, 
            audience: _jwtSettings.Audience, claims:claims, expires:expires, signingCredentials:creds);

        return new LoginResponse
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            UserId = user.Id,
            FullName = user.UserName,
            Role = user.Role.Name,
            ExpiresAt = expires
        };
    }
}