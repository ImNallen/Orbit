using Domain.Abstractions;
using Domain.Locations.Enums;
using MediatR;

namespace Application.Locations.Queries.GetLocationById;

/// <summary>
/// Query to get a location by ID.
/// </summary>
public sealed record GetLocationByIdQuery(Guid LocationId) : IRequest<Result<LocationDto, DomainError>>;

/// <summary>
/// Location data transfer object.
/// </summary>
public sealed record LocationDto(
    Guid LocationId,
    string Name,
    LocationType Type,
    LocationStatus Status,
    string Street,
    string City,
    string State,
    string Country,
    string ZipCode,
    bool IsOperational,
    DateTime CreatedAt,
    DateTime UpdatedAt);

