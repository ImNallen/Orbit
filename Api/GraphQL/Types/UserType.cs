using Api.GraphQL.Payloads;

namespace Api.GraphQL.Types;

/// <summary>
/// GraphQL type for the current authenticated user with role and permissions.
/// </summary>
public sealed class UserType
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public bool IsEmailVerified { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public string? Role { get; init; }
    public IReadOnlyList<string> Permissions { get; init; } = Array.Empty<string>();
    public Guid? CurrentLocationContextId { get; init; }
    public IReadOnlyList<UserAssignedLocationType> AssignedLocations { get; init; } = Array.Empty<UserAssignedLocationType>();
}

/// <summary>
/// GraphQL type for user's assigned location.
/// </summary>
public sealed class UserAssignedLocationType
{
    public Guid LocationId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Type { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public string City { get; init; } = string.Empty;
    public string? State { get; init; }
    public string Country { get; init; } = string.Empty;
    public bool IsPrimaryLocation { get; init; }
    public bool IsCurrentContext { get; init; }
}

/// <summary>
/// GraphQL type for user summary (without permissions).
/// </summary>
public sealed class UserSummaryType
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public bool IsEmailVerified { get; init; }
    public string Status { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? LastLoginAt { get; init; }
    public string? Role { get; init; }
}

/// <summary>
/// GraphQL payload for users query with pagination.
/// </summary>
public sealed class UsersPayload
{
    public IReadOnlyList<UserSummaryType> Users { get; init; } = Array.Empty<UserSummaryType>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();
}

