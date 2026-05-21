using Discounts.Application.Commands.Admin.Offers;
using Discounts.Application.Exceptions.OfferExceptions;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Handlers.Admin.Offers;

public class RejectOfferHandler(IOfferRepository offerRepository)
    : IRequestHandler<RejectOfferCommand>
{
    public async Task Handle(RejectOfferCommand command, CancellationToken ct =  default)
    {
        var existingOffer = await offerRepository.GetWithDetailsByIdAsync(command.OfferId, ct);
        
        if(existingOffer is null)
            throw new OfferNotFoundException("Offer not found !");
        
        if(existingOffer.StatusId != (int)OfferStatusesEnum.Pending)
            throw new ApplicationException("Only pending offers can be rejected !");
        
        existingOffer.StatusId = (int)OfferStatusesEnum.Rejected;
        existingOffer.RejectionReason = command.Reason;
        existingOffer.RejectedAt = DateTime.UtcNow;
        await offerRepository.SaveChangesAsync(ct);
    }
}