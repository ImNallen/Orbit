using Domain.Abstractions;
using Domain.Users.ValueObjects;

namespace Infrastructure.Authentication;

/// <summary>
/// Password hashing service using BCrypt.
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    private const int WorkFactor = 12; // BCrypt work factor (higher = more secure but slower)

    public PasswordHash Hash(Password password)
    {
        string? hash = BCrypt.Net.BCrypt.HashPassword(password.Value, WorkFactor);
        return PasswordHash.Create(hash);
    }

    public bool Verify(Password password, PasswordHash passwordHash)
    {
        try
        {
            return BCrypt.Net.BCrypt.Verify(password.Value, passwordHash.Value);
        }
        catch
        {
            // If verification fails for any reason (invalid hash format, etc.), return false
            return false;
        }
    }
}

