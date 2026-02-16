using System.Linq.Expressions;
using Discounts.Infra.Persistence;
using Dsicounts.Application.Interfaces;
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

    public Task<bool> ExistsAsync(Expression<Func<T, bool>> pred, CancellationToken ct = default)
    {
        return _dbSet.AsNoTracking().AnyAsync(pred, ct);
    }

    public virtual async Task SaveChangesAsync(CancellationToken ct = default) =>  await _context.SaveChangesAsync(ct);
}