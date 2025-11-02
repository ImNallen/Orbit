namespace Application.Users.Queries.GetUsers;

/// <summary>
/// Result of getting users with pagination.
/// </summary>
public sealed record GetUsersResult(
    IReadOnlyList<UserDto> Users,
    int TotalCount,
    int Page,
    int PageSize,
    int TotalPages);

/// <summary>
/// User data transfer object.
/// </summary>
public sealed record UserDto(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    bool IsEmailVerified,
    string Status,
    DateTime CreatedAt,
    DateTime? LastLoginAt,
    string? RoleName);

