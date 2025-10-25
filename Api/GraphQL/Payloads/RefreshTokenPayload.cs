using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for refresh token mutation.
/// </summary>
public sealed class RefreshTokenPayload
{
    public UserSummaryType? User { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static RefreshTokenPayload Success(UserSummaryType user, string accessToken, string refreshToken, DateTime expiresAt)
    {
        return new RefreshTokenPayload
        {
            User = user,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            Errors = Array.Empty<UserError>()
        };
    }

    public static RefreshTokenPayload Failure(params UserError[] errors)
    {
        return new RefreshTokenPayload
        {
            User = null,
            AccessToken = null,
            RefreshToken = null,
            ExpiresAt = null,
            Errors = errors
        };
    }
}

