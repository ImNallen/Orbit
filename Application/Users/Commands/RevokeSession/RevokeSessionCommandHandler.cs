using Domain.Abstractions;
using Domain.Users.Errors;
using Domain.Users.Session;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.RevokeSession;

/// <summary>
/// Handler for RevokeSessionCommand.
/// </summary>
public sealed class RevokeSessionCommandHandler : IRequestHandler<RevokeSessionCommand, Result<RevokeSessionResult, DomainError>>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<RevokeSessionCommandHandler> _logger;

    public RevokeSessionCommandHandler(
        ISessionRepository sessionRepository,
        ILogger<RevokeSessionCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _logger = logger;
    }

    public async Task<Result<RevokeSessionResult, DomainError>> Handle(
        RevokeSessionCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Revoking session {SessionId} for user {UserId}", command.SessionId, command.UserId);

        // 1. Get the session
        Session? session = await _sessionRepository.GetByIdAsync(command.SessionId, cancellationToken);
        if (session is null)
        {
            _logger.LogWarning("Session {SessionId} not found", command.SessionId);
            return Result<RevokeSessionResult, DomainError>.Failure(UserErrors.SessionNotFound);
        }

        // 2. Verify the session belongs to the user
        if (session.UserId != command.UserId)
        {
            _logger.LogWarning("User {UserId} attempted to revoke session {SessionId} belonging to user {SessionUserId}",
                command.UserId, command.SessionId, session.UserId);
            return Result<RevokeSessionResult, DomainError>.Failure(UserErrors.SessionNotFound);
        }

        // 3. Check if already revoked
        if (session.IsRevoked)
        {
            _logger.LogInformation("Session {SessionId} is already revoked", command.SessionId);
            return Result<RevokeSessionResult, DomainError>.Success(
                new RevokeSessionResult(true, "Session already revoked"));
        }

        // 4. Revoke the session
        session.Revoke();
        await _sessionRepository.UpdateAsync(session, cancellationToken);

        _logger.LogInformation("Session {SessionId} revoked successfully for user {UserId}", command.SessionId, command.UserId);

        return Result<RevokeSessionResult, DomainError>.Success(
            new RevokeSessionResult(true, "Session revoked successfully"));
    }
}

