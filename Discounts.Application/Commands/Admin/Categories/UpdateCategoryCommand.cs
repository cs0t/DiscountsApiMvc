using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Domain.Constants;
using MediatR;

namespace Discounts.Application.Commands.Admin.Categories;

public sealed record UpdateCategoryCommand(int Id, string NewName, string?  NewDescription):IRequest,IRequireRole
{
    public RoleEnum RoleRequired => RoleEnum.Administrator;
}


