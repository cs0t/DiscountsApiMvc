namespace Discounts.Application.Commands;

public class UpdateOfferCommand : CreateOfferCommand
{   
    public int OfferId { get; init; }
}