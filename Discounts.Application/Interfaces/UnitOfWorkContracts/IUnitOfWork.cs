namespace Discounts.Application.Interfaces.UnitOfWorkContracts;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken ct = default);
    Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default);
}