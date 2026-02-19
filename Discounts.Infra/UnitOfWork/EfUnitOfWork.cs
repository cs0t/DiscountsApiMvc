using Discounts.Application.Interfaces.UnitOfWorkContracts;
using Discounts.Infra.Persistence;

namespace Discounts.Infra.UnitOfWork;

public class EfUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    
    public EfUnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
    }

    public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync(ct);
        try
        {
            await action();
            await SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);
        }
        catch
        {
            await transaction.RollbackAsync(ct);
            throw;
        }
    }
}