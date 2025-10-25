using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for roles query.
/// </summary>
public sealed class RolesPayload
{
    public IReadOnlyList<RoleType> Roles { get; init; } = Array.Empty<RoleType>();
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static RolesPayload Success(IReadOnlyList<RoleType> roles) => new()
    {
        Roles = roles
    };

    public static RolesPayload Failure(params UserError[] errors) => new()
    {
        Errors = errors
    };
}

