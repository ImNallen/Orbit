using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.UnassignUserFromLocation;

/// <summary>
/// Command to unassign a user from a location.
/// </summary>
public sealed record UnassignUserFromLocationCommand(
    Guid UserId,
    Guid LocationId) : IRequest<Result<UnassignUserFromLocationResult, DomainError>>;

/// <summary>
/// Result of unassigning a user from a location.
/// </summary>
public sealed record UnassignUserFromLocationResult(string Message);

