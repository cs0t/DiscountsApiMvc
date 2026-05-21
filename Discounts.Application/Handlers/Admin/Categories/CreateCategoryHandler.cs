using Discounts.Application.Commands.Admin.Categories;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Domain.Entities;
using MediatR;

namespace Discounts.Application.Handlers.Admin.Categories;

public class CreateCategoryHandler(ICategoryRepository categoryRepository)
    : IRequestHandler<CreateCategoryCommand, int>
{
    public async Task<int> Handle(CreateCategoryCommand command, CancellationToken ct = default)
    { 
        if (await categoryRepository.ExistsAsync(c=>c.Name == command.Name, ct))
            throw new ApplicationException("Category already exists !");

        var category = new Category{Name = command.Name, Description = command.Description};

        await categoryRepository.Add(category, ct);
        await categoryRepository.SaveChangesAsync(ct);
        return category.Id;
    }
}