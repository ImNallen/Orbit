using Api.GraphQL.Types;

namespace Api.GraphQL.Payloads;

/// <summary>
/// Payload for permissions query.
/// </summary>
public sealed class PermissionsPayload
{
    public IReadOnlyList<PermissionType> Permissions { get; init; } = Array.Empty<PermissionType>();
    public IReadOnlyList<UserError> Errors { get; init; } = Array.Empty<UserError>();

    public static PermissionsPayload Success(IReadOnlyList<PermissionType> permissions) => new()
    {
        Permissions = permissions
    };

    public static PermissionsPayload Failure(params UserError[] errors) => new()
    {
        Errors = errors
    };
}

