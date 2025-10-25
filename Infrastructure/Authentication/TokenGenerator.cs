using System.Security.Cryptography;
using Domain.Abstractions;

namespace Infrastructure.Authentication;

/// <summary>
/// Token generator service using cryptographically secure random number generation.
/// </summary>
public class TokenGenerator : ITokenGenerator
{
    private const int TokenLengthInBytes = 32; // 256 bits

    public string GenerateToken()
    {
        byte[] randomBytes = new byte[TokenLengthInBytes];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        
        // Convert to URL-safe base64 string
        return Convert.ToBase64String(randomBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}

