using Discounts.Application.Commands;
using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.OfferExceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.AdminModuleContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;

namespace Discounts.Application.Services.AdminModuleServices;

public class OfferManagementAdminService : IOfferManagementAdminService
{
    private readonly IUserRepository _userRepository;
    private readonly IOfferRepository _offerRepository;

    public OfferManagementAdminService(IUserRepository userRepository, IOfferRepository offerRepository)
    {
        _userRepository = userRepository;
        _offerRepository = offerRepository;
    }
    
    public async Task ApproveOfferAsync(int adminId, int offerId, CancellationToken ct = default)
    {
        var admin = await _userRepository.GetWithRolesAsync(adminId, ct);
        
        if(admin is null)
            throw new UserNotFoundException("Admin not found");

        if (admin.RoleId != (int)RoleEnum.Administrator)
            throw new ForbiddenException("This user doesn`t have admin permissions !");
        
        var existingOffer = await _offerRepository.GetWithDetailsByIdAsync(offerId, ct);
        
        if(existingOffer is null)
            throw new OfferNotFoundException("Offer not found !");
        
        if(existingOffer.StatusId != (int)OfferStatusesEnum.Pending)
            throw new ApplicationException("Only pending offers can be approved !");
        
        if(existingOffer.ExpirationDate < DateTime.UtcNow)
            throw new ApplicationException("Cannot approve an expired offer !");
        
        existingOffer.StatusId = (int)OfferStatusesEnum.Approved;
        existingOffer.ApprovedAt = DateTime.UtcNow;
        await _offerRepository.SaveChangesAsync(ct);
    }

    public async Task RejectOfferAsync(int adminId, RejectOfferCommand command, CancellationToken ct = default)
    {
        var offerId = command.OfferId;
        var admin = await _userRepository.GetWithRolesAsync(adminId, ct);
        
        if(admin is null)
            throw new UserNotFoundException("Admin not found");

        if (admin.RoleId != (int)RoleEnum.Administrator)
            throw new ForbiddenException("This user doesn`t have admin permissions !");
        
        var existingOffer = await _offerRepository.GetWithDetailsByIdAsync(offerId, ct);
        
        if(existingOffer is null)
            throw new OfferNotFoundException("Offer not found !");
        
        if(existingOffer.StatusId != (int)OfferStatusesEnum.Pending)
            throw new ApplicationException("Only pending offers can be rejected !");
        
        existingOffer.StatusId = (int)OfferStatusesEnum.Rejected;
        existingOffer.RejectionReason = command.Reason;
        existingOffer.RejectedAt = DateTime.UtcNow;
        await _offerRepository.SaveChangesAsync(ct);
    }
    
    public async Task<PagedResult<Offer>> GetOffersPagedForAdminAsync(int adminId,int pageNumber = 1, int pageSize = 8,
        CancellationToken ct = default)
    {
        var admin = await _userRepository.GetWithRolesAsync(adminId, ct);
            
        if(admin is null)
            throw new UserNotFoundException("Admin not found");
            
        if (admin.RoleId != (int)RoleEnum.Administrator)
            throw new ForbiddenException("This user doesn`t have admin permissions !");
        return await _offerRepository.GetPagedAsync(pageNumber, pageSize, ct);
    }
}