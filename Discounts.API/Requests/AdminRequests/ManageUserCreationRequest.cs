namespace Discounts.API.Requests.AdminRequests;

public class ManageUserCreationRequest
{
    public string UserName { get; init; } = null!;
    public string Email { get; init; } = null!;
    public string Password { get; init; } = null!;
    public string ConfirmPassword { get; init; } = null!;
    public int RoleId { get; init; }
}