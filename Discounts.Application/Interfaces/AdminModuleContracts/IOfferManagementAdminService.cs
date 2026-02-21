using Discounts.Application.Commands;
using Discounts.Application.Models;
using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.AdminModuleContracts;

public interface IOfferManagementAdminService
{
    public Task ApproveOfferAsync(int adminId, int offerId, CancellationToken ct = default);
    public Task RejectOfferAsync(int adminId, RejectOfferCommand command, CancellationToken ct = default);

    Task<PagedResult<Offer>> GetOffersPagedForAdminAsync(int adminId, int pageNumber = 1, int pageSize = 8,
        CancellationToken ct = default);
}