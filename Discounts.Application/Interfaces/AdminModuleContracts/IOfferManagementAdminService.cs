using Discounts.Application.Commands;

namespace Discounts.Application.Interfaces.AdminModuleContracts;

public interface IOfferManagementAdminService
{
    public Task ApproveOfferAsync(int adminId, int offerId, CancellationToken ct = default);
    public Task RejectOfferAsync(int adminId, RejectOfferCommand command, CancellationToken ct = default);
}