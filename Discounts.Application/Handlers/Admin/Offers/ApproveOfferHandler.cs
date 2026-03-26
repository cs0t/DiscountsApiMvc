using Discounts.Application.Commands.Admin.Offers;
using Discounts.Application.Exceptions.OfferExceptions;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Handlers.Admin.Offers;

public class ApproveOfferHandler(IOfferRepository offerRepository) 
    : IRequestHandler<ApproveOfferCommand>
{
    public async Task Handle(ApproveOfferCommand command, CancellationToken ct = default)
    {
        var existingOffer = await offerRepository.GetWithDetailsByIdAsync(command.Id, ct);
        
        if(existingOffer is null)
            throw new OfferNotFoundException("Offer not found !");
        
        if(existingOffer.StatusId != (int)OfferStatusesEnum.Pending)
            throw new ApplicationException("Only pending offers can be approved !");
        
        if(existingOffer.ExpirationDate < DateTime.UtcNow)
            throw new ApplicationException("Cannot approve an expired offer !");
        
        existingOffer.StatusId = (int)OfferStatusesEnum.Approved;
        existingOffer.ApprovedAt = DateTime.UtcNow;
        await offerRepository.SaveChangesAsync(ct);
    }
}