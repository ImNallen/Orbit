namespace Application.Users.Commands.RevokeSession;

/// <summary>
/// Result for RevokeSessionCommand.
/// </summary>
public sealed record RevokeSessionResult(bool IsSuccess, string Message);

