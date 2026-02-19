namespace Discounts.Application.Queries;

public class OfferListQuery
{
    public decimal? PriceStart { get; set; }
    public decimal? PriceEnd { get; set; }
    public IReadOnlyCollection<int>? CategoryIds { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 8;
}