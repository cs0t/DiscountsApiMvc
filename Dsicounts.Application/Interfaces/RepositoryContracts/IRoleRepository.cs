using Discounts.Domain.Entities;

namespace Dsicounts.Application.Interfaces.RepositoryContracts;

public interface IRoleRepository : IRepository<Role>
{
    Task<Role?> GetByName(string roleName,CancellationToken ct = default);
    Task<List<Role>> GetAll(CancellationToken ct = default);
}