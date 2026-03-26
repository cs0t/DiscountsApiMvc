using Discounts.Application.Commands.Admin.Categories;
using Discounts.Application.Exceptions.CategoryExceptions;
using Discounts.Application.Interfaces.RepositoryContracts;
using MediatR;

namespace Discounts.Application.Handlers.Admin.Categories;

public class DeleteCategoryHandler(ICategoryRepository categoryRepository) 
    : IRequestHandler<DeleteCategoryCommand>
{
    public async Task Handle(DeleteCategoryCommand command, CancellationToken ct = default)
    {
        var existingCategory = await categoryRepository.GetById(command.Id, ct);
        if (existingCategory is null)
            throw new CategoryNotFoundException("Category not found !");
        
        categoryRepository.Delete(existingCategory);
        await categoryRepository.SaveChangesAsync(ct);
    }
}