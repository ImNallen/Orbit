using Application.Abstractions.Behaviors;
using Domain.Abstractions;
using HotChocolate;

namespace Api.GraphQL.ErrorFilters;

/// <summary>
/// GraphQL error filter that converts domain errors and validation errors
/// into user-friendly GraphQL errors with proper error codes and extensions.
/// </summary>
public sealed class DomainErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        // Get the exception from the error
        Exception? exception = error.Exception;

        // If there's no exception, return the error as-is
        if (exception is null)
        {
            return error;
        }

        // Handle different types of exceptions
        return exception switch
        {
            // Domain validation errors (from FluentValidation)
            _ when exception.Message.Contains("Validation.Failed") => HandleValidationError(error, exception),
            
            // Unauthorized access
            UnauthorizedAccessException => CreateError(
                error,
                "UNAUTHORIZED",
                "You are not authorized to perform this action",
                "AUTH_ERROR"),
            
            // Invalid operation
            InvalidOperationException => CreateError(
                error,
                "INVALID_OPERATION",
                exception.Message,
                "BUSINESS_ERROR"),
            
            // Argument exceptions
            ArgumentException => CreateError(
                error,
                "INVALID_ARGUMENT",
                exception.Message,
                "VALIDATION_ERROR"),
            
            // Default: return original error
            _ => error
        };
    }

    /// <summary>
    /// Handles validation errors from FluentValidation.
    /// </summary>
    private static IError HandleValidationError(IError error, Exception exception)
    {
        IErrorBuilder errorBuilder = ErrorBuilder.New()
            .SetMessage("One or more validation errors occurred")
            .SetCode("VALIDATION_ERROR")
            .SetPath(error.Path);

        // Add all locations from the original error
        foreach (Location location in error.Locations ?? [])
        {
            errorBuilder.AddLocation(location);
        }

        // Try to extract validation details from the exception message
        errorBuilder.SetExtension("validationErrors", exception.Message);

        return errorBuilder.Build();
    }

    /// <summary>
    /// Creates a standardized error with code and category.
    /// </summary>
    private static IError CreateError(
        IError originalError,
        string code,
        string message,
        string category)
    {
        IErrorBuilder errorBuilder = ErrorBuilder.New()
            .SetMessage(message)
            .SetCode(code)
            .SetPath(originalError.Path);

        // Add all locations from the original error
        foreach (Location location in originalError.Locations ?? [])
        {
            errorBuilder.AddLocation(location);
        }

        errorBuilder.SetExtension("category", category);

        return errorBuilder.Build();
    }
}

