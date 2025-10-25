using Domain.Abstractions;

namespace Infrastructure.Time;

/// <summary>
/// DateTime provider implementation that returns the current UTC time.
/// </summary>
public class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow => DateTime.UtcNow;
}

