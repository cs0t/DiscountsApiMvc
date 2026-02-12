namespace Discounts.Domain.Entities;

public class OfferStatus
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    
    public ICollection<Offer> Offers { get; set; } = new List<Offer>();
}