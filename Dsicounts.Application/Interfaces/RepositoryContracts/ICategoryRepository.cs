using Discounts.Domain.Entities;

namespace Dsicounts.Application.Interfaces.RepositoryContracts;

public interface ICategoryRepository : IRepository<Category>
{
    Task<List<Category>> GetAllCategoriesAsync(CancellationToken ct = default);
}