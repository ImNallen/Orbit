using Domain.Abstractions;
using MediatR;

namespace Application.Users.Commands.AssignUserToLocation;

/// <summary>
/// Command to assign a user to a location.
/// </summary>
public sealed record AssignUserToLocationCommand(
    Guid UserId,
    Guid LocationId,
    Guid? LocationRoleId = null,
    bool IsPrimaryLocation = false) : IRequest<Result<AssignUserToLocationResult, DomainError>>;

/// <summary>
/// Result of assigning a user to a location.
/// </summary>
public sealed record AssignUserToLocationResult(
    Guid AssignmentId,
    Guid UserId,
    Guid LocationId,
    string Message);

