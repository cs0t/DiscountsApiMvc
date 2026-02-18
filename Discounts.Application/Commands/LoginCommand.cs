namespace Discounts.Application.Commands;

public class LoginCommand
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}