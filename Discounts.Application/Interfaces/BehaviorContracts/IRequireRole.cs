using Discounts.Domain.Constants;

namespace Discounts.Application.Interfaces.BehaviorContracts;

public interface IRequireRole
{
    public RoleEnum RoleRequired { get; }
}