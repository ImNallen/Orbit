using Domain.Abstractions;
using Domain.Session;
using Domain.Shared.ValueObjects;
using Domain.Users;
using Domain.Users.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.Login;

/// <summary>
/// Handler for LoginCommand.
/// </summary>
public sealed class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IAuthorizationService _authorizationService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ITokenExpirationSettings _tokenExpirationSettings;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(
        IUserRepository userRepository,
        ISessionRepository sessionRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService,
        IAuthorizationService authorizationService,
        IHttpContextAccessor httpContextAccessor,
        ITokenExpirationSettings tokenExpirationSettings,
        ILogger<LoginCommandHandler> logger)
    {
        _userRepository = userRepository;
        _sessionRepository = sessionRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _authorizationService = authorizationService;
        _httpContextAccessor = httpContextAccessor;
        _tokenExpirationSettings = tokenExpirationSettings;
        _logger = logger;
    }

    public async Task<Result<LoginResult, DomainError>> Handle(
        LoginCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Login attempt for email: {Email} from IP: {IpAddress}",
            command.Email, _httpContextAccessor.IpAddress);

        // 1. Create Email value object
        Result<Email, DomainError> emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
        {
            _logger.LogWarning("Login failed: Invalid email format for {Email}", command.Email);
            return Result<LoginResult, DomainError>.Failure(UserErrors.InvalidCredentials);
        }

        Email email = emailResult.Value;

        // 2. Find user by email
        User? user = await _userRepository.GetByEmailAsync(email, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("Login failed: User not found for email {Email}", command.Email);
            return Result<LoginResult, DomainError>.Failure(UserErrors.InvalidCredentials);
        }

        // 3. Validate user can login (checks email verification, account status, and lockout)
        Result<DomainError> canLoginResult = user.ValidateCanLogin();
        if (canLoginResult.IsFailure)
        {
            _logger.LogWarning("Login failed for user {UserId}: {ErrorCode} - {ErrorMessage}",
                user.Id, canLoginResult.Error.Code, canLoginResult.Error.Message);
            return Result<LoginResult, DomainError>.Failure(canLoginResult.Error);
        }

        // 4. Verify password
        Result<Password, DomainError> passwordResult = Password.Create(command.Password);
        if (passwordResult.IsFailure)
        {
            _logger.LogWarning("Login failed for user {UserId}: Invalid password format from IP {IpAddress}",
                user.Id, _httpContextAccessor.IpAddress);
            user.RecordFailedLogin(_httpContextAccessor.IpAddress);
            await _userRepository.UpdateAsync(user, cancellationToken);
            return Result<LoginResult, DomainError>.Failure(UserErrors.InvalidCredentials);
        }

        bool isPasswordValid = _passwordHasher.Verify(passwordResult.Value, user.PasswordHash);
        if (!isPasswordValid)
        {
            _logger.LogWarning("Login failed for user {UserId}: Invalid password from IP {IpAddress}",
                user.Id, _httpContextAccessor.IpAddress);
            user.RecordFailedLogin(_httpContextAccessor.IpAddress);
            await _userRepository.UpdateAsync(user, cancellationToken);
            return Result<LoginResult, DomainError>.Failure(UserErrors.InvalidCredentials);
        }

        // 5. Load user role and permissions
        string? role = await _authorizationService.GetUserRoleAsync(user.Id, cancellationToken);
        List<string> permissions = await _authorizationService.GetUserPermissionsAsync(user.Id, cancellationToken);

        _logger.LogDebug("User {UserId} has role {Role} and {PermissionCount} permissions",
            user.Id, role ?? "none", permissions.Count);

        // 6. Generate tokens
        string refreshToken = _jwtTokenService.GenerateRefreshToken();
        IEnumerable<string> roles = role != null ? [role] : [];
        string accessToken = _jwtTokenService.GenerateAccessToken(
            user.Id,
            user.Email.Value,
            roles,
            permissions);

        // 7. Create session
        var session = Session.Create(
            user.Id,
            refreshToken,
            _httpContextAccessor.IpAddress,
            _httpContextAccessor.UserAgent,
            TimeSpan.FromDays(_tokenExpirationSettings.RefreshTokenExpirationDays));

        await _sessionRepository.AddAsync(session, cancellationToken);

        // 8. Record successful login
        user.RecordSuccessfulLogin(session.Id, _httpContextAccessor.IpAddress);
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("Login successful for user {UserId} ({Email}) from IP {IpAddress}",
            user.Id, user.Email.Value, _httpContextAccessor.IpAddress);

        // 9. Return success result
        return Result<LoginResult, DomainError>.Success(
            new LoginResult(
                user.Id,
                user.Email.Value,
                user.FullName.FirstName,
                user.FullName.LastName,
                accessToken,
                refreshToken,
                DateTime.UtcNow.AddMinutes(_tokenExpirationSettings.AccessTokenExpirationMinutes)));
    }
}

