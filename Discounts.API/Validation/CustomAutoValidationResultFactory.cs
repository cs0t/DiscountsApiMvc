using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharpGrip.FluentValidation.AutoValidation.Mvc.Results;

namespace Discounts.API.Validation;

public class CustomAutoValidationResultFactory : IFluentValidationAutoValidationResultFactory
{
    public Task<IActionResult?> CreateActionResult(
        ActionExecutingContext context,
        ValidationProblemDetails validationProblemDetails,
        IDictionary<IValidationContext, ValidationResult> validationResults)
    {
        var errors = validationProblemDetails.Errors
            .SelectMany(e => e.Value.Select(msg => $"{e.Key}: {msg}"))
            .ToList();
        //nromlize fluentvalidation error responses
        var errorResponse = new ProblemDetails
        {
            Status = StatusCodes.Status400BadRequest,
            Title = "Validation Error!",
            Type = "https://httpstatuses.com/400",
            Detail = string.Join("; ", errors),
            Instance = context.HttpContext.Request.Path,
        };
        errorResponse.Extensions["traceId"] = context.HttpContext.TraceIdentifier;

        return Task.FromResult<IActionResult?>(new BadRequestObjectResult(errorResponse));
    }
}

