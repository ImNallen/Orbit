using Domain.Abstractions;
using MediatR;

namespace Application.Locations.Commands.RemoveLocationManager;

/// <summary>
/// Command to remove the manager from a location.
/// </summary>
public sealed record RemoveLocationManagerCommand(
    Guid LocationId) : IRequest<Result<RemoveLocationManagerResult, DomainError>>;

/// <summary>
/// Result of removing the manager from a location.
/// </summary>
public sealed record RemoveLocationManagerResult(string Message);

