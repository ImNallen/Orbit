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
    IReadOnlyList<string> Permissions);

