using Application.Users.Queries.GetUsers;

namespace Application.Users.Queries.GetUsersByLocation;

/// <summary>
/// Result of getting users by location with pagination.
/// </summary>
public sealed record GetUsersByLocationResult(
    IReadOnlyList<UserDto> Users,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);

