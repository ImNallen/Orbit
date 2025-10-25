using Domain.Users.User;

namespace Domain.Abstractions;

/// <summary>
/// Service for hashing and verifying passwords.
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hashes a plain-text password.
    /// </summary>
    /// <param name="password">The plain-text password to hash.</param>
    /// <returns>The hashed password.</returns>
    PasswordHash Hash(Password password);

    /// <summary>
    /// Verifies that a plain-text password matches a hashed password.
    /// </summary>
    /// <param name="password">The plain-text password to verify.</param>
    /// <param name="passwordHash">The hashed password to compare against.</param>
    /// <returns>True if the password matches, otherwise false.</returns>
    bool Verify(Password password, PasswordHash passwordHash);
}

