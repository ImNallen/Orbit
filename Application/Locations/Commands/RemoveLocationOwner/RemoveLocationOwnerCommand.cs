using Domain.Abstractions;
using MediatR;

namespace Application.Locations.Commands.RemoveLocationOwner;

/// <summary>
/// Command to remove the owner from a location.
/// </summary>
public sealed record RemoveLocationOwnerCommand(
    Guid LocationId) : IRequest<Result<RemoveLocationOwnerResult, DomainError>>;

/// <summary>
/// Result of removing the owner from a location.
/// </summary>
public sealed record RemoveLocationOwnerResult(string Message);

