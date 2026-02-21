using System.Linq.Expressions;
using Discounts.Application.Models;

namespace Discounts.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetById(int id, CancellationToken ct = default);
    Task Add(T entity, CancellationToken ct = default);
    void Update(T entity);
    void Delete(T entity);
    Task<bool> ExistsAsync(Expression<Func<T,bool>> pred, CancellationToken ct = default);
    Task<List<T>> GetByPredicateAsync(Expression<Func<T, bool>> func, CancellationToken ct = default);
    Task<PagedResult<T>> GetPagedAsync(int pageNumber = 1 , int pageSize = 8, CancellationToken ct = default);
    Task SaveChangesAsync(CancellationToken ct = default);
}