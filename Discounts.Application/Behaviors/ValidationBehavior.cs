using FluentValidation;
using MediatR;

namespace Discounts.Application.Behaviors;

public class ValidationBehavior<TRequest,TResponse>
    (IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest,TResponse> 
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct = default)
    {
        if (!validators.Any())
            return await next(ct);
        
        var context = new ValidationContext<TRequest>(request);
        
        var results = await 
            Task.WhenAll(validators.Select(val => val.ValidateAsync(context,ct)));
        
        var errors = results
            .SelectMany(result => result.Errors).Where(failure => failure is not null ).ToList();
        
        if(errors.Count != 0)
            throw new ValidationException(errors);
        
        return await next(ct);
    }
}