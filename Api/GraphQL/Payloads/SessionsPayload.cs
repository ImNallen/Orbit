using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for sessions query.
/// </summary>
public sealed class SessionsPayload
{
    public IReadOnlyList<SessionType> Sessions { get; init; } = Array.Empty<SessionType>();
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static SessionsPayload Success(IReadOnlyList<SessionType> sessions) => new()
    {
        Sessions = sessions
    };

    public static SessionsPayload Failure(params UserError[] errors) => new()
    {
        Errors = errors
    };
}

