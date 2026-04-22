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
        try
        {
            await context.SaveChangesAsync(ct);
            if (_transaction is not null)
                await _transaction.CommitAsync(ct);
        }
        catch
        {
            if (_transaction is not null)
                await _transaction.RollbackAsync(ct);
            throw;
        }
        finally
        {
            if (_transaction is not null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await context.SaveChangesAsync(ct);
    }

    public async Task RollbackAsync(CancellationToken ct = default)
    {
        if (_transaction is not null)
        {
           await _transaction.RollbackAsync(ct);
           await _transaction.DisposeAsync();
           _transaction = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_transaction != null) await _transaction.DisposeAsync();
        await context.DisposeAsync();
    }
}