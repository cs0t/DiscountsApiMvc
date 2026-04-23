using Discounts.Application.Interfaces.UnitOfWorkContracts;
using Discounts.Infra.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Discounts.Infra.UnitOfWork;
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

public class EfUnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public EfUnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task BeginTransactionAsync(CancellationToken ct = default)
    {
        if (_transaction is not null) 
            throw new InvalidOperationException("The transaction has already started!");
        _transaction = await _context.Database.BeginTransactionAsync(ct);   
    }

    public async Task CommitAsync(CancellationToken ct = default)
    {
        if (_transaction is null)
            throw new InvalidOperationException("The are no active transactions!");
        
        try
        {
            await _context.SaveChangesAsync(ct);
            await _transaction.CommitAsync(ct);
        }
        catch
        {
            await _transaction.RollbackAsync(ct);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task SaveChangesAsync(CancellationToken ct = default)
    {
        await _context.SaveChangesAsync(ct);
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
        if (_transaction is not null)
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
        await _context.DisposeAsync();
    }
}