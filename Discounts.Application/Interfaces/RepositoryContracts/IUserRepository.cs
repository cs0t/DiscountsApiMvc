using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.RepositoryContracts;

public interface IUserRepository : IRepository<User>
{
    Task<User> AddAndReturnAsync(User entity, CancellationToken ct = default);
    Task<User?> GetByEmailAsync(string email, CancellationToken ct = default);
    Task<User?> GetWithRolesAsync(int id, CancellationToken ct = default);
}