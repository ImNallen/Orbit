using Api.GraphQL.Payloads;

namespace Api.GraphQL.Types;

/// <summary>
/// GraphQL type for the current authenticated user with roles and permissions.
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
    public IReadOnlyList<string> Roles { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Permissions { get; init; } = Array.Empty<string>();
}

/// <summary>
/// GraphQL type for user summary (without roles and permissions).
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

