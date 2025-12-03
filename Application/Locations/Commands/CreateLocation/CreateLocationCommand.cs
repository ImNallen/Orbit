using Domain.Abstractions;
using Domain.Locations.Enums;
using MediatR;

namespace Application.Locations.Commands.CreateLocation;

/// <summary>
/// Command to create a new location.
/// </summary>
public sealed record CreateLocationCommand(
    string Name,
    LocationType Type,
    string Street,
    string City,
    string? State,
    string Country,
    string ZipCode) : IRequest<Result<CreateLocationResult, DomainError>>;

/// <summary>
/// Result of location creation.
/// </summary>
public sealed record CreateLocationResult(
    Guid LocationId,
    string Name,
    LocationType Type,
    string City,
    string? State);

