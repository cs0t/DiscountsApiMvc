using Discounts.Infra.Persistence;

namespace Discounts.Infra.Repositories;

public class Repository<T> where T : class
{
    protected readonly ApplicationDbContext _context;

    public Repository(ApplicationDbContext dbContext)
    {
        _context = dbContext;
    }
    
    protected virtual async Task<T?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Set<T>().FindAsync(new object[] { id }, ct);
    }
    
    protected virtual async Task AddAsync(T entity, CancellationToken ct = default)
    {
        await _context.Set<T>().AddAsync(entity, ct);
    }
    
    protected virtual Task Update(T entity)
    {
        return Task.FromResult(_context.Set<T>().Update(entity));
    }

    protected virtual Task Delete(T entity, CancellationToken ct = default)
    {
        return Task.FromResult(_context.Set<T>().Remove(entity));
    }
}