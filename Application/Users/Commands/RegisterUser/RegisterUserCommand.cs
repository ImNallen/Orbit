using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.RegisterUser;

/// <summary>
/// Command to register a new user.
/// </summary>
public sealed record RegisterUserCommand(
    string Email,
    string Password,
    string FirstName,
    string LastName) : IRequest<Result<RegisterUserResult, DomainError>>;

/// <summary>
/// Result of user registration.
/// </summary>
public sealed record RegisterUserResult(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName);

