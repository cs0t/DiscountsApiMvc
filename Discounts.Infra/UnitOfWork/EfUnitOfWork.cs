using Discounts.Application.Interfaces.UnitOfWorkContracts;
using Discounts.Infra.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Discounts.Infra.UnitOfWork;

public class EfUnitOfWork(ApplicationDbContext context) : IUnitOfWork
{

    // public async Task SaveChangesAsync(CancellationToken ct = default)
    //     await _context.SaveChangesAsync(ct);
    // }
    //
    // public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default)
    // {
    //     await using var transaction = await _context.Database.BeginTransactionAsync(ct);
    //     try
    //     {
    //         await action();
    //         await SaveChangesAsync(ct);
    //         await transaction.CommitAsync(ct);
    //     }
    //     catch
    //     {
    //         await transaction.RollbackAsync(ct);
    //         throw;
    //     }
    // }
    
    private  IDbContextTransaction? _transaction;

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        _transaction = await context.Database.BeginTransactionAsync(ct);   
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        await context.SaveChangesAsync(ct);
        if(_transaction is not null)
            await _transaction.CommitAsync(ct);
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction is not null)
           await _transaction.RollbackAsync(ct);
    }
}