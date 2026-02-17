namespace Discounts.Application.Commands;

public class CreateOfferCommand
{
    public string Title { get; init; } = null!;
    public string? Description { get; init; } 
    public decimal OriginalPrice { get; init; }
    public decimal DiscountedPrice { get; init; }
    public int MaxQuantity { get; init; }
    public int RemainingQuantity { get; init; }
    //public DateTime CreatedAt { get; init; }
    public DateTime ExpirationDate { get; init; }
    public IReadOnlyCollection<int> CategoryIds { get; init; } = [];
}