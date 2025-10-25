using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for reset password mutation.
/// </summary>
public sealed class ResetPasswordPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static ResetPasswordPayload CreateSuccess(string message)
    {
        return new ResetPasswordPayload
        {
            IsSuccess = true,
            Message = message,
            Errors = Array.Empty<UserError>()
        };
    }

    public static ResetPasswordPayload CreateFailure(params UserError[] errors)
    {
        return new ResetPasswordPayload
        {
            IsSuccess = false,
            Errors = errors
        };
    }
}

