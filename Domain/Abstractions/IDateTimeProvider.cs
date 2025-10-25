namespace Domain.Abstractions;

/// <summary>
/// Abstraction for DateTime to enable testability.
/// </summary>
public interface IDateTimeProvider
{
    /// <summary>
    /// Gets the current UTC date and time.
    /// </summary>
    DateTime UtcNow { get; }
}

