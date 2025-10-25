using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for request password reset mutation.
/// </summary>
public sealed class RequestPasswordResetPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static RequestPasswordResetPayload CreateSuccess(string message)
    {
        return new RequestPasswordResetPayload
        {
            IsSuccess = true,
            Message = message,
            Errors = Array.Empty<UserError>()
        };
    }

    public static RequestPasswordResetPayload CreateFailure(params UserError[] errors)
    {
        return new RequestPasswordResetPayload
        {
            IsSuccess = false,
            Errors = errors
        };
    }
}

