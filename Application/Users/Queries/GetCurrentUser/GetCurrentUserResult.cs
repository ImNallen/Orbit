using Domain.Locations.Enums;

namespace Application.Users.Queries.GetCurrentUser;

/// <summary>
/// Result of getting the current user.
/// </summary>
public sealed record GetCurrentUserResult(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    bool IsEmailVerified,
    string Status,
    DateTime CreatedAt,
    DateTime? LastLoginAt,
    string? Role,
    IReadOnlyList<string> Permissions,
    Guid? CurrentLocationContextId,
    IReadOnlyList<UserLocationDto> AssignedLocations);

/// <summary>
/// User location assignment data transfer object.
/// </summary>
public sealed record UserLocationDto(
    Guid LocationId,
    string Name,
    LocationType Type,
    LocationStatus Status,
    string City,
    string? State,
    string Country,
    bool IsPrimaryLocation,
    bool IsCurrentContext);

