using System.Collections.Immutable;
using Discounts.Domain.Entities;
using Discounts.Infra.Persistence;
using Discounts.Application.Interfaces.RepositoryContracts;
using Microsoft.EntityFrameworkCore;

namespace Discounts.Infra.Repositories;

public class RoleRepository : Repository<Role>,IRoleRepository
{
    public RoleRepository(ApplicationDbContext dbContext) : base(dbContext) { }

    public Task<Role?> GetByName(string roleName, CancellationToken ct = default)
    {
        return _context.Roles.FirstOrDefaultAsync(r=>r.Name == roleName, ct);
    }

    public Task<List<Role>> GetAll(CancellationToken ct = default)
    {
        return _context.Roles.AsNoTracking().ToListAsync(ct);
    }
}