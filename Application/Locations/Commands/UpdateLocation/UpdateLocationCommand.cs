using Domain.Abstractions;
using Domain.Locations.Enums;
using MediatR;

namespace Application.Locations.Commands.UpdateLocation;

/// <summary>
/// Command to update a location's information.
/// </summary>
public sealed record UpdateLocationCommand(
    Guid LocationId,
    string Name,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode) : IRequest<Result<UpdateLocationResult, DomainError>>;

/// <summary>
/// Result of location update.
/// </summary>
public sealed record UpdateLocationResult(
    Guid LocationId,
    string Name);

