using System.Reflection;
using Domain.Abstractions;
using FluentValidation;
using FluentValidation.Results;
using MediatR;

namespace Application.Abstractions.Behaviors;

/// <summary>
/// MediatR pipeline behavior that validates commands using FluentValidation.
/// Runs before the command handler and returns validation errors if validation fails.
/// </summary>
public sealed class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : class
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidationBehavior(IEnumerable<IValidator<TRequest>> validators)
    {
        _validators = validators;
    }

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        // If no validators are registered, skip validation
        if (!_validators.Any())
        {
#pragma warning disable CA2016 // RequestHandlerDelegate doesn't accept CancellationToken
            return await next().ConfigureAwait(false);
#pragma warning restore CA2016
        }

        // Create validation context
        var context = new ValidationContext<TRequest>(request);

        // Run all validators
        ValidationResult[] validationResults = await Task.WhenAll(
            _validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        // Collect all validation failures
        List<ValidationFailure> failures = validationResults
            .SelectMany(r => r.Errors)
            .Where(f => f != null)
            .ToList();

        // If there are validation failures, return error response
        if (failures.Count != 0)
        {
            return CreateValidationErrorResponse(failures);
        }

        // Validation passed, continue to handler
#pragma warning disable CA2016 // RequestHandlerDelegate doesn't accept CancellationToken
        return await next().ConfigureAwait(false);
#pragma warning restore CA2016
    }

    /// <summary>
    /// Creates a validation error response from validation failures.
    /// This method uses reflection to create the appropriate Result type.
    /// </summary>
    private static TResponse CreateValidationErrorResponse(List<ValidationFailure> failures)
    {
        // Get the response type (should be Result<TValue, TError>)
        Type responseType = typeof(TResponse);

        // Check if it's a Result type
        if (!responseType.IsGenericType)
        {
            throw new InvalidOperationException(
                $"ValidationBehavior can only be used with Result<TValue, TError> return types. Got: {responseType.Name}");
        }

        Type genericTypeDefinition = responseType.GetGenericTypeDefinition();

        // Handle Result<TValue, TError>
        if (genericTypeDefinition == typeof(Result<,>))
        {
            Type[] genericArgs = responseType.GetGenericArguments();
            Type errorType = genericArgs[1]; // TError

            // Create a validation error
            object validationError = CreateValidationError(failures, errorType);

            // Call Result<TValue, TError>.Failure(error)
            MethodInfo? failureMethod = responseType.GetMethod("Failure", new[] { errorType }) ?? throw new InvalidOperationException(
                    $"Could not find Failure method on {responseType.Name}");

            object? result = failureMethod.Invoke(null, new[] { validationError });
            return (TResponse)result!;
        }

        // Handle Result<TError>
        if (genericTypeDefinition == typeof(Result<>))
        {
            Type errorType = responseType.GetGenericArguments()[0]; // TError

            // Create a validation error
            object validationError = CreateValidationError(failures, errorType);

            // Call Result<TError>.Failure(error)
            MethodInfo? failureMethod = responseType.GetMethod("Failure", new[] { errorType }) ?? throw new InvalidOperationException(
                    $"Could not find Failure method on {responseType.Name}");

            object? result = failureMethod.Invoke(null, new[] { validationError });
            return (TResponse)result!;
        }

        throw new InvalidOperationException(
            $"ValidationBehavior can only be used with Result<TValue, TError> or Result<TError> return types. Got: {responseType.Name}");
    }

    /// <summary>
    /// Creates a validation error from validation failures.
    /// </summary>
    private static object CreateValidationError(List<ValidationFailure> failures, Type errorType)
    {
        // Build error message
        string errorMessage = string.Join("; ", failures.Select(f => f.ErrorMessage));

        // Check if errorType is DomainError or a subtype
        if (errorType == typeof(DomainError) || errorType.IsSubclassOf(typeof(DomainError)))
        {
            // Create a ValidationError (which is a DomainError)
            return new ValidationError(
                "Validation.Failed",
                errorMessage,
                failures.ToDictionary(f => f.PropertyName, f => f.ErrorMessage));
        }

        // If it's not a DomainError, try to create it using a constructor
        // that takes (string code, string message)
        ConstructorInfo? constructor = errorType.GetConstructor(new[] { typeof(string), typeof(string) });
        if (constructor != null)
        {
            return constructor.Invoke(new object[] { "Validation.Failed", errorMessage })!;
        }

        throw new InvalidOperationException(
            $"Could not create validation error of type {errorType.Name}. " +
            "Error type must be DomainError or have a constructor that takes (string code, string message)");
    }
}

/// <summary>
/// Validation error that includes detailed field-level errors.
/// </summary>
public sealed record ValidationError : DomainError
{
    public ValidationError(string code, string message, Dictionary<string, string> errors)
        : base(code, message)
    {
        Errors = errors;
    }

    public Dictionary<string, string> Errors { get; }
}

