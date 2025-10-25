namespace Application.Users.Queries.GetUserById;

/// <summary>
/// Result of getting a user by ID.
/// </summary>
public sealed record GetUserByIdResult(
    Guid UserId,
    string Email,
    string FirstName,
    string LastName,
    bool IsEmailVerified,
    string Status,
    DateTime CreatedAt,
    DateTime? LastLoginAt);

