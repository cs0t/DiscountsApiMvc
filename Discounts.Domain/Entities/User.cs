namespace Discounts.Domain.Entities;

public class User
{
    public int Id { get; set; }
    public string UserName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;
    
    public int RoleId { get; set; }
    public Role? Role { get; set; }
}