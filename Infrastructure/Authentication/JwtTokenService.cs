using System.Globalization;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Domain.Abstractions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;
using JwtSecurityToken = System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
using JwtSecurityTokenHandler = System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler;

namespace Infrastructure.Authentication;

/// <summary>
/// JWT token service for generating access and refresh tokens.
/// </summary>
public class JwtTokenService : IJwtTokenService
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly int _accessTokenExpirationMinutes;

    public JwtTokenService(IConfiguration configuration)
    {
        _secret = configuration["Jwt:Secret"] ?? throw new InvalidOperationException("JWT Secret is not configured");
        _issuer = configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");
        _audience = configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured");
        _accessTokenExpirationMinutes = int.Parse(configuration["Jwt:AccessTokenExpirationMinutes"] ?? "60", CultureInfo.InvariantCulture);
    }

    public string GenerateAccessToken(Guid userId, string email, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new(JwtRegisteredClaimNames.Email, email),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, userId.ToString())
        };

        // Add role claims
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

        // Add permission claims
        claims.AddRange(permissions.Select(permission => new Claim("permission", permission)));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_accessTokenExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken()
    {
        byte[] randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        
        return Convert.ToBase64String(randomBytes)
            .Replace('+', '-')
            .Replace('/', '_')
            .TrimEnd('=');
    }
}

