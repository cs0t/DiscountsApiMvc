using Discounts.Application.Models;
using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.JwtContracts;

public interface IJwtService
{
    LoginResponse GenerateToken(User user);
}