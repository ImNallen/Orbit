namespace Api.GraphQL.Inputs;

/// <summary>
/// Input for changePassword mutation.
/// </summary>
public sealed record ChangePasswordInput(
    string CurrentPassword,
    string NewPassword);

