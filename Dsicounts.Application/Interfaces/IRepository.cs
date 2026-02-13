namespace Dsicounts.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task AddAsync(T entity, CancellationToken ct = default);
    Task UpdateAsync(T entity, CancellationToken ct = default);
    Task DeleteAsync(T entity, CancellationToken ct = default);
    Task<T> GetByIdAsync(int id, CancellationToken ct = default);
}