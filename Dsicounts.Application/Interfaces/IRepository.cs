namespace Dsicounts.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetById(int id, CancellationToken ct = default);
    Task Add(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Delete(T entity);
    Task SaveChangesAsync(CancellationToken ct = default);
}