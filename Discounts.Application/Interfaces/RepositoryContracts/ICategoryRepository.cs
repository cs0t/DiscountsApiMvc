using Discounts.Domain.Entities;

namespace Discounts.Application.Interfaces.RepositoryContracts;

public interface ICategoryRepository : IRepository<Category>
{
    Task<List<Category>> GetAllCategoriesAsync(CancellationToken ct = default);
}