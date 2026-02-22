using Discounts.Application.Interfaces.SystemSettingsContracts;
using Discounts.Domain.Constants;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Discounts.Infra.Persistence;

namespace Discounts.WorkerService;

public class CleanupService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<CleanupService> _logger;
    
    public CleanupService(IServiceScopeFactory serviceScopeFactory, ILogger<CleanupService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
    }
    
    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                var settingServices = scope.ServiceProvider.GetRequiredService<ISystemSettingsService>();

                var cycleInterval =
                    await settingServices.GetSettingValueByKeyAsync<int>(
                        SystemSettingNames.CleanupServiceCycleInMinutes, ct);
                
                var now = DateTime.UtcNow;
                //start transaction
                using var transaction = await context.Database.BeginTransactionAsync(ct);
                
                try 
                {
                    //restore offer quants
                    //count of expired reservations grouped by offer
                    var targetOffers = await context.Reservations
                        .Where(o => o.IsActive && o.ValidUntil <= now)
                        .GroupBy(o => o.OfferId)
                        .Select(g => new { OfferId = g.Key, Count = g.Count() })
                        .ToListAsync(ct);
                    
                    //restore quantities 
                    foreach (var offer in targetOffers)
                    {
                        await context.Offers
                            .Where(o => o.Id == offer.OfferId)
                            .ExecuteUpdateAsync(setters =>
                                setters.SetProperty(o=>o.RemainingQuantity, o=>o.RemainingQuantity + offer.Count)
                                ,ct);
                    }
                    
                    //deactivate expired reservations
                    var expiredReservationsCnt = await context.Reservations
                        .Where(r => r.IsActive && r.ValidUntil <= now)
                        .ExecuteUpdateAsync(setters =>
                            setters.SetProperty(r => r.IsActive, false)
                                .SetProperty(r => r.CancelledAt, now), ct);
                    
                    _logger.LogInformation($"Deactivated {expiredReservationsCnt} reservations !");
                    
                    //expire offers
                    var expiredOffersCnt = await context.Offers
                        .Where(o=> o.StatusId != (int)OfferStatusesEnum.Expired && o.ExpirationDate <= now)
                        .ExecuteUpdateAsync(setters => 
                            setters.SetProperty(o => o.StatusId, (int)OfferStatusesEnum.Expired), ct);
                    
                    _logger.LogInformation($"Expired {expiredOffersCnt} offers !");

                    await transaction.CommitAsync(ct);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync(ct);
                    throw;
                }
                
                await Task.Delay(TimeSpan.FromMinutes(cycleInterval), ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Cleanup cycle failed !");
                await Task.Delay(TimeSpan.FromMinutes(5), ct);
            }
        }
    }
}