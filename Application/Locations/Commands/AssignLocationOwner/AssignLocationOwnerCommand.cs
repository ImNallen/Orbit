using Domain.Abstractions;
using MediatR;

namespace Application.Locations.Commands.AssignLocationOwner;

/// <summary>
/// Command to assign an owner to a location.
/// </summary>
public sealed record AssignLocationOwnerCommand(
    Guid LocationId,
    Guid UserId) : IRequest<Result<AssignLocationOwnerResult, DomainError>>;

/// <summary>
/// Result of assigning an owner to a location.
/// </summary>
public sealed record AssignLocationOwnerResult(string Message);

