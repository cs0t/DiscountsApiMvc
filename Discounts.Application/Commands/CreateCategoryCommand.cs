namespace Discounts.Application.Commands;

public class CreateCategoryCommand
{
    public string Name { get; init; } = null!;
    public string? Description { get; init; }
}