namespace Domain.Abstractions;

/// <summary>
/// Service for generating and validating JWT tokens.
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generates a JWT access token for the specified user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="email">The user's email address.</param>
    /// <param name="roles">The user's roles.</param>
    /// <param name="permissions">The user's permissions.</param>
    /// <returns>A JWT access token string.</returns>
    string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles, IEnumerable<string> permissions);

    /// <summary>
    /// Generates a refresh token.
    /// </summary>
    /// <returns>A secure refresh token string.</returns>
    string GenerateRefreshToken();
}

