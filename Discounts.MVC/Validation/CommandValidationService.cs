using FluentValidation;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Discounts.MVC.Validation;

/// Validates Application-layer command objects using their registered FluentValidation validators
/// maps any errors into MVC ModelState so controllers can return views with proper error messages.
public interface ICommandValidationService
{
    Task<bool> ValidateAndAddErrorsAsync<T>(T command, ModelStateDictionary modelState, CancellationToken ct = default);
}

public class CommandValidationService : ICommandValidationService
{
    private readonly IServiceProvider _serviceProvider;

    public CommandValidationService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<bool> ValidateAndAddErrorsAsync<T>(T command, ModelStateDictionary modelState, CancellationToken ct = default)
    {
        var validator = _serviceProvider.GetService<IValidator<T>>();
        if (validator == null)
            return true; // No validator registered — nothing to check

        var result = await validator.ValidateAsync(command, ct);
        if (result.IsValid)
            return true;

        foreach (var error in result.Errors)
        {
            modelState.AddModelError(error.PropertyName, error.ErrorMessage);
        }

        return false;
    }
}

