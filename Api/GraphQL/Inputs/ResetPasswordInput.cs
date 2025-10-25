namespace Api.GraphQL.Inputs;

/// <summary>
/// Input for resetting a password.
/// </summary>
public sealed record ResetPasswordInput(
    string Token,
    string NewPassword);

