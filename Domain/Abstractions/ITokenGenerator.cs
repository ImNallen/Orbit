namespace Domain.Abstractions;

/// <summary>
/// Service for generating secure tokens (email verification, password reset, etc.).
/// </summary>
public interface ITokenGenerator
{
    /// <summary>
    /// Generates a cryptographically secure random token.
    /// </summary>
    /// <returns>A secure token string.</returns>
    string GenerateToken();
}

