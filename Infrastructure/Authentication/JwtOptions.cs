namespace Infrastructure.Authentication;

/// <summary>
/// Configuration options for JWT tokens.
/// </summary>
public sealed class JwtOptions
{
    /// <summary>
    /// Configuration section name.
    /// </summary>
    public const string SectionName = "Jwt";

    /// <summary>
    /// JWT secret key for signing tokens.
    /// </summary>
    public string Secret { get; set; } = string.Empty;

    /// <summary>
    /// JWT issuer.
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// JWT audience.
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Access token expiration in minutes.
    /// </summary>
    public int AccessTokenExpirationMinutes { get; set; } = 60;

    /// <summary>
    /// Refresh token expiration in days.
    /// </summary>
    public int RefreshTokenExpirationDays { get; set; } = 7;
}

