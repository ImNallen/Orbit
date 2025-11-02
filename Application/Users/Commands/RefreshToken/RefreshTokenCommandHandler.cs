using Domain.Abstractions;
using Domain.Session;
using Domain.Users;
using Domain.Users.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.RefreshToken;

/// <summary>
/// Handler for RefreshTokenCommand.
/// </summary>
public sealed class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, Result<RefreshTokenResult, DomainError>>
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IUserRepository _userRepository;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IAuthorizationService _authorizationService;
    private readonly ITokenExpirationSettings _tokenExpirationSettings;
    private readonly ILogger<RefreshTokenCommandHandler> _logger;

    public RefreshTokenCommandHandler(
        ISessionRepository sessionRepository,
        IUserRepository userRepository,
        IJwtTokenService jwtTokenService,
        IAuthorizationService authorizationService,
        ITokenExpirationSettings tokenExpirationSettings,
        ILogger<RefreshTokenCommandHandler> logger)
    {
        _sessionRepository = sessionRepository;
        _userRepository = userRepository;
        _jwtTokenService = jwtTokenService;
        _authorizationService = authorizationService;
        _tokenExpirationSettings = tokenExpirationSettings;
        _logger = logger;
    }

    public async Task<Result<RefreshTokenResult, DomainError>> Handle(
        RefreshTokenCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Token refresh attempt");

        // 1. Validate refresh token is not empty
        if (string.IsNullOrWhiteSpace(command.RefreshToken))
        {
            _logger.LogWarning("Token refresh failed: Empty refresh token");
            return Result<RefreshTokenResult, DomainError>.Failure(UserErrors.InvalidRefreshToken);
        }

        // 2. Find session by refresh token
        Session? session = await _sessionRepository.GetByRefreshTokenAsync(command.RefreshToken, cancellationToken);
        if (session is null)
        {
            _logger.LogWarning("Token refresh failed: Session not found for refresh token");
            return Result<RefreshTokenResult, DomainError>.Failure(UserErrors.InvalidRefreshToken);
        }

        // 3. Validate session (check if expired or revoked)
        Result<DomainError> validationResult = session.Validate();
        if (validationResult.IsFailure)
        {
            _logger.LogWarning("Token refresh failed for session {SessionId}: {ErrorCode} - {ErrorMessage}",
                session.Id, validationResult.Error.Code, validationResult.Error.Message);
            return Result<RefreshTokenResult, DomainError>.Failure(validationResult.Error);
        }

        // 4. Get user
        User? user = await _userRepository.GetByIdAsync(session.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogError("Token refresh failed: User {UserId} not found for session {SessionId}",
                session.UserId, session.Id);
            return Result<RefreshTokenResult, DomainError>.Failure(UserErrors.UserNotFound);
        }

        // 5. Check account status
        if (user.Status == UserStatus.Suspended)
        {
            _logger.LogWarning("Token refresh failed for user {UserId}: Account suspended", user.Id);
            return Result<RefreshTokenResult, DomainError>.Failure(UserErrors.AccountSuspended);
        }

        if (user.Status == UserStatus.Deleted)
        {
            _logger.LogWarning("Token refresh failed for user {UserId}: Account deleted", user.Id);
            return Result<RefreshTokenResult, DomainError>.Failure(UserErrors.AccountDeleted);
        }

        // 6. Revoke the old session (refresh token rotation)
        session.Revoke();
        await _sessionRepository.UpdateAsync(session, cancellationToken);

        _logger.LogDebug("Revoked old session {SessionId} for user {UserId}", session.Id, user.Id);

        // 7. Load user role and permissions
        string? role = await _authorizationService.GetUserRoleAsync(user.Id, cancellationToken);
        List<string> permissions = await _authorizationService.GetUserPermissionsAsync(user.Id, cancellationToken);

        // 8. Generate new tokens (both access and refresh)
        IEnumerable<string> roles = role != null ? [role] : [];
        string newAccessToken = _jwtTokenService.GenerateAccessToken(user.Id, user.Email.Value, roles, permissions);
        string newRefreshToken = _jwtTokenService.GenerateRefreshToken();

        // 9. Create new session with new refresh token
        var newSession = Session.Create(
            user.Id,
            newRefreshToken,
            session.IpAddress, // Keep same IP address from old session
            session.UserAgent, // Keep same user agent from old session
            TimeSpan.FromDays(_tokenExpirationSettings.RefreshTokenExpirationDays));

        await _sessionRepository.AddAsync(newSession, cancellationToken);

        _logger.LogInformation("Token refresh successful for user {UserId} ({Email}). New session {SessionId} created",
            user.Id, user.Email.Value, newSession.Id);

        // 10. Return success result with new tokens
        return Result<RefreshTokenResult, DomainError>.Success(
            new RefreshTokenResult(
                user.Id,
                user.Email.Value,
                newAccessToken,
                newRefreshToken, // Return the NEW refresh token
                DateTime.UtcNow.AddMinutes(_tokenExpirationSettings.AccessTokenExpirationMinutes)));
    }
}

