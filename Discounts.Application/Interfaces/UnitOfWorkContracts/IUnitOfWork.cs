namespace Discounts.Application.Interfaces.UnitOfWorkContracts;

public interface IUnitOfWork : IAsyncDisposable
{
    Task BeginTransactionAsync(CancellationToken ct = default);
    Task CommitAsync(CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
    Task RollbackAsync(CancellationToken ct = default);
}