using Discounts.Domain.Entities;
using Discounts.Infra.Persistence;
using Discounts.Application.Interfaces.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infra.Repositories;

public class UserRepository : Repository<User>, IUserRepository
{
    public UserRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public Task<User?> GetByEmailAsync(string email, CancellationToken ct = default)
    {
        return _context.Users.FirstOrDefaultAsync(u => u.Email == email, ct);
    }

    public Task<User?> GetWithRolesAsync(int id, CancellationToken ct = default)
    {
        return _context.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.Id == id, ct);
    }
}