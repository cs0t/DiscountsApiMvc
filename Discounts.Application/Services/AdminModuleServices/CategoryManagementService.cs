using Discounts.Application.Commands;
using Discounts.Application.Exceptions;
using Discounts.Application.Exceptions.CategoryExceptions;
using Discounts.Application.Exceptions.UserExceptions;
using Discounts.Application.Interfaces.AdminModuleContracts;
using Discounts.Application.Interfaces.RepositoryContracts;
using Discounts.Application.Models;
using Discounts.Domain.Constants;
using Discounts.Domain.Entities;

namespace Discounts.Application.Services.AdminModuleServices;

public class CategoryManagementService : ICategoryManagementService
{
    private readonly IUserRepository _userRepository;
    private readonly ICategoryRepository _categoryRepository;

    public CategoryManagementService(IUserRepository userRepository, ICategoryRepository categoryRepository)
    {
        _userRepository = userRepository;
        _categoryRepository = categoryRepository;
    }
    
    public async Task<int> CreateCategoryAsync(int adminId, CreateCategoryCommand command ,CancellationToken ct = default)
    {
        var admin = await _userRepository.GetWithRolesAsync(adminId, ct);
        
        if(admin is null)
            throw new UserNotFoundException("Admin not found");

        if (admin.RoleId != (int)RoleEnum.Administrator)
            throw new ForbiddenException("This user doesn`t have admin permissions !");

        if (await _categoryRepository.ExistsAsync(c=>c.Name == command.Name, ct))
            throw new ApplicationException("Category already exists");

        var category = new Category{Name = command.Name, Description = command.Description};

        await _categoryRepository.Add(category, ct);
        await _categoryRepository.SaveChangesAsync(ct);
        return category.Id;
    }

    public async Task UpdateCategoryAsync(int adminId, UpdateCategoryCommand command, CancellationToken ct = default)
    {
        var admin = await _userRepository.GetWithRolesAsync(adminId, ct);
        
        if(admin is null)
            throw new UserNotFoundException("Admin not found");

        if (admin.RoleId != (int)RoleEnum.Administrator)
            throw new ForbiddenException("This user doesn`t have admin permissions !");
                   

        var existingCategory = await _categoryRepository.GetById(command.Id, ct);
        if (existingCategory is null)
            throw new CategoryNotFoundException("Category not found !");

        existingCategory.Name = command.NewName;
        if(!string.IsNullOrWhiteSpace(command.NewDescription))
            existingCategory.Description = command.NewDescription;

        await _categoryRepository.SaveChangesAsync(ct);
    }
    
        public async Task DeleteCategoryAsync(int adminId, int categoryId, CancellationToken ct = default)
        {
            var admin = await _userRepository.GetWithRolesAsync(adminId, ct);
            
            if(admin is null)
                throw new UserNotFoundException("Admin not found");
        
            if (admin.RoleId != (int)RoleEnum.Administrator)
                throw new ForbiddenException("This user doesn`t have admin permissions !");
        
            var existingCategory = await _categoryRepository.GetById(categoryId, ct);
            if (existingCategory is null)
                throw new CategoryNotFoundException("Category not found !");
        
            _categoryRepository.Delete(existingCategory);
            await _categoryRepository.SaveChangesAsync(ct);
        }

        public async Task<PagedResult<Category>> GetCategoriesPagedForAdminAsync(int adminId,int pageNumber = 1, int pageSize = 8,
            CancellationToken ct = default)
        {
            var admin = await _userRepository.GetWithRolesAsync(adminId, ct);
            
            if(admin is null)
                throw new UserNotFoundException("Admin not found");
            
            if (admin.RoleId != (int)RoleEnum.Administrator)
                throw new ForbiddenException("This user doesn`t have admin permissions !");
            return await _categoryRepository.GetPagedAsync(pageNumber, pageSize, ct);
        }
}