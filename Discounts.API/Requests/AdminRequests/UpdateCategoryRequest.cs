namespace Discounts.API.Requests.AdminRequests;

public class UpdateCategoryRequest
{
    public int Id { get; init; }
    public string NewName { get; init; } = null!;
    public string? NewDescription { get; init; }
}