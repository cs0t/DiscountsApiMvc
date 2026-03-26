using Discounts.Application.Commands.AdminCommands;
using Discounts.Application.Exceptions.CategoryExceptions;
using Discounts.Application.Interfaces.RepositoryContracts;
using MediatR;

namespace Discounts.Application.Handlers.Admin;

public class UpdateCategoryHandler(ICategoryRepository categoryRepository) 
    : IRequestHandler<UpdateCategoryCommand>
{
    public async Task Handle(UpdateCategoryCommand command, CancellationToken ct = default)
    {
        var existingCategory = await categoryRepository.GetById(command.Id, ct);
        if (existingCategory is null)
            throw new CategoryNotFoundException("Category not found !");

        existingCategory.Name = command.NewName;
        if(!string.IsNullOrWhiteSpace(command.NewDescription))
            existingCategory.Description = command.NewDescription;

        await categoryRepository.SaveChangesAsync(ct);
    }
}