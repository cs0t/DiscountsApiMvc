namespace Discounts.Application.Commands;

public class ManageUserUpdateCommand
{
    public int UserId { get; set; }
    public string? UserName { get; init; } = null!;
    public string? Email { get; init; } = null!;
    public string? Password { get; init; } = null!;
    public string? ConfirmPassword { get; init; } = null!;
    public int? RoleId { get; init; }
}

