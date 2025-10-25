using Domain.Abstractions;
using Domain.Users.Session;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.RevokeAllSessions;

/// <summary>
/// Handler for RevokeAllSessionsCommand.
/// </summary>
public sealed class RevokeAllSessionsCommandHandler : IRequestHandler<RevokeAllSessionsCommand, Result<RevokeAllSessionsResult, DomainError>>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<RevokeAllSessionsCommandHandler> _logger;

    public RevokeAllSessionsCommandHandler(
        ISessionRepository sessionRepository,
        ILogger<RevokeAllSessionsCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    public async Task<Result<RevokeAllSessionsResult, DomainError>> Handle(
        RevokeAllSessionsCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Revoking all sessions for user {UserId} except current session {CurrentSessionId}",
            command.UserId, command.CurrentSessionId);

        // 1. Get all active sessions for the user
        List<Session> sessions = await _sessionRepository.GetActiveSessionsByUserIdAsync(command.UserId, cancellationToken);

        // 2. Filter out the current session if specified
        List<Session> sessionsToRevoke = command.CurrentSessionId.HasValue
            ? sessions.Where(s => s.Id != command.CurrentSessionId.Value).ToList()
            : sessions;

        // 3. Revoke all sessions
        int revokedCount = 0;
        foreach (Session session in sessionsToRevoke)
        {
            if (!session.IsRevoked)
            {
                session.Revoke();
                await _sessionRepository.UpdateAsync(session, cancellationToken);
                revokedCount++;
            }
        }

        _logger.LogInformation("Revoked {RevokedCount} sessions for user {UserId}", revokedCount, command.UserId);

        string message = revokedCount == 0
            ? "No sessions to revoke"
            : $"Successfully revoked {revokedCount} session(s)";

        return Result<RevokeAllSessionsResult, DomainError>.Success(
            new RevokeAllSessionsResult(true, message, revokedCount));
    }
}

