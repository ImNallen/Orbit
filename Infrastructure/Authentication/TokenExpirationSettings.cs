using Domain.Abstractions;
using Microsoft.Extensions.Options;

namespace Infrastructure.Authentication;

/// <summary>
/// Implementation of ITokenExpirationSettings that reads from configuration.
/// </summary>
public sealed class TokenExpirationSettings : ITokenExpirationSettings
{
    private readonly JwtOptions _jwtOptions;

    public TokenExpirationSettings(IOptions<JwtOptions> jwtOptions)
    {
        _jwtOptions = jwtOptions.Value;
    }

    public int AccessTokenExpirationMinutes => _jwtOptions.AccessTokenExpirationMinutes;

    public int RefreshTokenExpirationDays => _jwtOptions.RefreshTokenExpirationDays;
}

