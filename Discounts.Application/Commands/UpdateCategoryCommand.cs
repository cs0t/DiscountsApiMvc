namespace Discounts.Application.Commands;

public class UpdateCategoryCommand
{
    public int Id { get; init; }
    public string NewName { get; init; } = null!;
    public string? NewDescription { get; init; }
}

