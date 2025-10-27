using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.UpdateUserProfile;

/// <summary>
/// Command to update a user's profile information.
/// </summary>
public sealed record UpdateUserProfileCommand(
    Guid UserId,
    string FirstName,
    string LastName) : IRequest<Result<UpdateUserProfileResult, DomainError>>;

/// <summary>
/// Result of updating user profile.
/// </summary>
public sealed record UpdateUserProfileResult(
    Guid UserId,
    string FirstName,
    string LastName,
    string Message);

