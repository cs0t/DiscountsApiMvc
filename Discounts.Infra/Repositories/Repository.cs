using System.Linq.Expressions;
using Discounts.Infra.Persistence;
using Discounts.Application.Interfaces;
using Discounts.Application.Models;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infra.Repositories;

public abstract class Repository<T> : IRepository<T> where T : class
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext dbContext)
    {
        _context = dbContext;
        _dbSet = _context.Set<T>();
    }
    
    public virtual async Task<T?> GetById(int id, CancellationToken ct = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, ct);
    }
    
    public virtual async Task Add(T entity, CancellationToken ct = default)
    {
        await _dbSet.AddAsync(entity, ct);
    }
    
    public virtual void Update(T entity) => _dbSet.Update(entity);

    public virtual void Delete(T entity) => _dbSet.Remove(entity);

    public virtual Task<bool> ExistsAsync(Expression<Func<T, bool>> pred, CancellationToken ct = default)
    {
        return _dbSet.AsNoTracking().AnyAsync(pred, ct);
    }

    public virtual Task<List<T>> GetByPredicateAsync(Expression<Func<T, bool>> func,
        CancellationToken ct = default)
    {
        return _dbSet.Where(func).ToListAsync(ct);
    }

    public virtual async Task<PagedResult<T>> GetPagedAsync(int pageNumber = 1, int pageSize = 8, CancellationToken ct = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize is < 1 or > 20 ? 8 : pageSize;
        
        var query = _dbSet.AsNoTracking();
        
        var totalCount = await query.CountAsync(ct);
        var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new  PagedResult<T>(items, totalCount, pageNumber, pageSize);
    }

    public virtual async Task SaveChangesAsync(CancellationToken ct = default) =>  await _context.SaveChangesAsync(ct);
}