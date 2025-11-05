using Domain.Abstractions;
using Domain.Locations.Enums;
using MediatR;

namespace Application.Locations.Commands.ChangeLocationStatus;

/// <summary>
/// Command to change a location's status.
/// </summary>
public sealed record ChangeLocationStatusCommand(
    Guid LocationId,
    LocationStatus NewStatus) : IRequest<Result<ChangeLocationStatusResult, DomainError>>;

/// <summary>
/// Result of location status change.
/// </summary>
public sealed record ChangeLocationStatusResult(
    Guid LocationId,
    LocationStatus Status);

