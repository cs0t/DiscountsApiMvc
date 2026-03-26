using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Commands.Admin.Categories;

public sealed record DeleteCategoryCommand(int id) : IRequest,IRequireRole,IEntityByIdCommand
{
    public RoleEnum RoleRequired => RoleEnum.Administrator;
    public int Id => id;
}
