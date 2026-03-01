using Bintainer.Common.Domain;
using FluentValidation;
using MediatR;

namespace Bintainer.Common.Application.Behaviors;

internal sealed class ValidationPipelineBehavior<TRequest, TResponse>(
    IEnumerable<IValidator<TRequest>> validators)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var validationFailures = await ValidateAsync(request);

        if (validationFailures.Length > 0)
        {
            if (typeof(TResponse).IsGenericType &&
                typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var resultType = typeof(TResponse).GetGenericArguments()[0];
                var failureMethod = typeof(Result).GetMethods()
                    .First(m => m.Name == nameof(Result.Failure) && m.IsGenericMethod)
                    .MakeGenericMethod(resultType);

                return (TResponse)failureMethod.Invoke(null, [new ValidationError(validationFailures)])!;
            }

            if (typeof(TResponse) == typeof(Result))
            {
                return (TResponse)(object)Result.Failure(new ValidationError(validationFailures));
            }

            throw new ValidationException(validationFailures.Select(e =>
                new FluentValidation.Results.ValidationFailure(e.Code, e.Description)));
        }

        return await next();
    }

    private async Task<Error[]> ValidateAsync(TRequest request)
    {
        if (!validators.Any())
        {
            return [];
        }

        var context = new ValidationContext<TRequest>(request);

        var validationResults = await Task.WhenAll(
            validators.Select(v => v.ValidateAsync(context)));

        var errors = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f is not null)
            .Select(f => Error.Failure(f.PropertyName, f.ErrorMessage))
            .Distinct()
            .ToArray();

        return errors;
    }
}
