using Domain.Abstractions;
using MediatR;

namespace Application.Locations.Commands.AssignLocationManager;

/// <summary>
/// Command to assign a manager to a location.
/// </summary>
public sealed record AssignLocationManagerCommand(
    Guid LocationId,
    Guid UserId) : IRequest<Result<AssignLocationManagerResult, DomainError>>;

/// <summary>
/// Result of assigning a manager to a location.
/// </summary>
public sealed record AssignLocationManagerResult(string Message);

