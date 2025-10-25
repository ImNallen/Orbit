namespace Domain.Abstractions;

/// <summary>
/// Contains validation-related domain errors.
/// </summary>
public static class ValidationErrors
{
    public static DomainError InvalidPage => new ValidationError(
        "Validation.InvalidPage",
        "Page must be greater than 0.");

    public static DomainError InvalidPageSize => new ValidationError(
        "Validation.InvalidPageSize",
        "Page size must be between 1 and 100.");

    // Private error record
    private sealed record ValidationError(string Code, string Message) : DomainError(Code, Message);
}

