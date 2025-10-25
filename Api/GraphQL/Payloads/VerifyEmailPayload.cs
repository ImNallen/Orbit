using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for email verification mutation.
/// </summary>
public sealed class VerifyEmailPayload
{
    public bool IsSuccess { get; init; }
    public string? Message { get; init; }
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static VerifyEmailPayload CreateSuccess(string message)
    {
        return new VerifyEmailPayload
        {
            IsSuccess = true,
            Message = message,
            Errors = Array.Empty<UserError>()
        };
    }

    public static VerifyEmailPayload CreateFailure(params UserError[] errors)
    {
        return new VerifyEmailPayload
        {
            IsSuccess = false,
            Errors = errors
        };
    }
}

