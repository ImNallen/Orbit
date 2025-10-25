using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for login mutation.
/// </summary>
public sealed class LoginPayload
{
    public UserSummaryType? User { get; init; }
    public string? AccessToken { get; init; }
    public string? RefreshToken { get; init; }
    public DateTime? ExpiresAt { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static LoginPayload Success(UserSummaryType user, string accessToken, string refreshToken, DateTime expiresAt)
    {
        return new LoginPayload
        {
            User = user,
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            Errors = Array.Empty<UserError>()
        };
    }

    public static LoginPayload Failure(params UserError[] errors)
    {
        return new LoginPayload
        {
            User = null,
            AccessToken = null,
            RefreshToken = null,
            ExpiresAt = null,
            Errors = errors
        };
    }
}

