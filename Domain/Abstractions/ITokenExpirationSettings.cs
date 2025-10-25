namespace Domain.Abstractions;

/// <summary>
/// Abstraction for token expiration settings.
/// </summary>
public interface ITokenExpirationSettings
{
    /// <summary>
    /// Gets the access token expiration in minutes.
    /// </summary>
    int AccessTokenExpirationMinutes { get; }

    /// <summary>
    /// Gets the refresh token expiration in days.
    /// </summary>
    int RefreshTokenExpirationDays { get; }
}

