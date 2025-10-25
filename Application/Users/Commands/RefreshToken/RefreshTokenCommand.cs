using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.RefreshToken;

/// <summary>
/// Command to refresh an access token using a refresh token.
/// </summary>
public sealed record RefreshTokenCommand(
    string RefreshToken) : IRequest<Result<RefreshTokenResult, DomainError>>;

/// <summary>
/// Result of a successful token refresh operation.
/// </summary>
public sealed record RefreshTokenResult(
    Guid UserId,
    string Email,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);

