using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for user registration mutation.
/// </summary>
public sealed class RegisterUserPayload
{
    /// <summary>
    /// The registered user (null if registration failed).
    /// </summary>
    public UserSummaryType? User { get; init; }

    /// <summary>
    /// List of errors that occurred during registration.
    /// </summary>
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    /// <summary>
    /// Creates a successful registration payload.
    /// </summary>
    public static RegisterUserPayload Success(UserSummaryType user) =>
        new() { User = user };

    /// <summary>
    /// Creates a failed registration payload.
    /// </summary>
    public static RegisterUserPayload Failure(params UserError[] errors) =>
        new() { Errors = errors };
}

/// <summary>
/// Represents a user-facing error.
/// </summary>
public sealed class UserError
{
    public string Code { get; init; }
    public string Message { get; init; }

    public UserError(string code, string message)
    {
        Code = code;
        Message = message;
    }
}

