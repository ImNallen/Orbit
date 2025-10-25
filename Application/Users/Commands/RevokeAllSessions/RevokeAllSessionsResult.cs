namespace Application.Users.Commands.RevokeAllSessions;

/// <summary>
/// Result for RevokeAllSessionsCommand.
/// </summary>
public sealed record RevokeAllSessionsResult(bool IsSuccess, string Message, int RevokedCount);

