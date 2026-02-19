namespace Discounts.API.Requests.AdminRequests;

public class CreateCategoryRequest
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}