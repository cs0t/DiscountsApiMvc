using Discounts.Application.Interfaces.BehaviorContracts;
using Discounts.Application.Interfaces.UnitOfWorkContracts;
using MediatR;

namespace Discounts.Application.Behaviors;

public class TransactionBehavior<TRequest,TResponse>(IUnitOfWork uow)
    : IPipelineBehavior<TRequest,TResponse>  
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct = default)
    {
        if (request is not ITransactionalCommand)
            return await next(ct);

        await uow.BeginTransactionAsync(ct);

        try
        {
            var response = await next(ct);
            await uow.CommitAsync(ct);
            return response;
        }
        catch
        {
            await uow.RollbackAsync(ct);
            throw;
        }
    }
}