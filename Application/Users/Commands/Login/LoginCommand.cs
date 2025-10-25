using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.Login;

/// <summary>
/// Command to authenticate a user and create a session.
/// </summary>
public sealed record LoginCommand(
    string Email,
    string Password) : IRequest<Result<LoginResult, DomainError>>;

/// <summary>
/// Result of user login.
/// </summary>
public sealed record LoginResult(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt);

