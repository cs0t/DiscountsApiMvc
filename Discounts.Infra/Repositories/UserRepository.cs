using Discounts.Application.Models;
using Discounts.Domain.Entities;
using Discounts.Infra.Persistence;
using Discounts.Application.Interfaces.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infra.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public override async Task<PagedResult<User>> GetPagedAsync(int pageNumber = 1, int pageSize = 8, CancellationToken ct = default)
    {
        pageNumber = pageNumber < 1 ? 1 : pageNumber;
        pageSize = pageSize is < 1 or > 20 ? 8 : pageSize;

        var query = _context.Users.AsNoTracking();
        var totalCount = await query.CountAsync(ct);
        var items = await query
            .Include(u => u.Role)
            .Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<User>(items, totalCount, pageNumber, pageSize);
    }

    public async Task<User> AddAndReturnAsync(User entity, CancellationToken ct = default)
    {
        await base.Add(entity,ct);
        await base.SaveChangesAsync(ct);
        
        return await _context.Users.Include(u=>u.Role).FirstAsync(u => u.Id == entity.Id, ct);
    }
    
    public async Task<User> UpdateAndReturnAsync(User entity, CancellationToken ct = default)
    {
        base.Update(entity);
        await base.SaveChangesAsync(ct);
        
        return await _context.Users.Include(u=>u.Role).FirstAsync(u => u.Id == entity.Id, ct);
    }
    
    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return _context.Users.Include(u=>u.Role).FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public Task<User?> GetWithRolesAsync(int id, CancellationToken ct = default)
    {
        return _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }
}