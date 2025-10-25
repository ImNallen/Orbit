using Domain.Abstractions;
using Domain.Users.Errors;
using Domain.Users.Role;

namespace Domain.Users.User;

/// <summary>
/// Represents a user in the system.
/// </summary>
public sealed class User : Entity
{
    private const int MaxFailedLoginAttempts = 5;
    private static readonly TimeSpan AccountLockoutDuration = TimeSpan.FromMinutes(15);
    private const int PasswordHistoryLimit = 5;

    private readonly List<Guid> _roleIds = [];
    private readonly List<PasswordHash> _passwordHistory = [];

    private User(
        Guid id,
        Email email,
        PasswordHash passwordHash,
        FullName fullName)
        : base(id)
    {
        Email = email;
        PasswordHash = passwordHash;
        FullName = fullName;
        Status = UserStatus.Active;
        IsEmailVerified = false;
        FailedLoginAttempts = 0;
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // EF Core constructor
    private User() { }

    // Profile Information
    public Email Email { get; private set; } = null!;
    public PasswordHash PasswordHash { get; private set; } = null!;
    public FullName FullName { get; private set; } = null!;

    // Account Status
    public UserStatus Status { get; private set; }
    public bool IsEmailVerified { get; private set; }

    // Security
    public int FailedLoginAttempts { get; private set; }
    public DateTime? LockedUntil { get; private set; }
    public string? PasswordResetToken { get; private set; }
    public DateTime? PasswordResetTokenExpiresAt { get; private set; }
    public string? EmailVerificationToken { get; private set; }
    public DateTime? EmailVerificationTokenExpiresAt { get; private set; }

    // Timestamps
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public DateTime? LastLoginAt { get; private set; }

    // Relationships
    public IReadOnlyCollection<Guid> RoleIds => _roleIds.AsReadOnly();
    public IReadOnlyCollection<PasswordHash> PasswordHistory => _passwordHistory.AsReadOnly();

    /// <summary>
    /// Creates a new user.
    /// </summary>
    public static Result<User, DomainError> Create(
        Email email,
        PasswordHash passwordHash,
        FullName fullName)
    {
        var user = new User(
            Guid.NewGuid(),
            email,
            passwordHash,
            fullName);

        // Add initial password to history
        user._passwordHistory.Add(passwordHash);

        user.Raise(new UserRegisteredEvent(
            user.Id,
            user.Email.Value,
            user.FullName.FirstName,
            user.FullName.LastName));

        return Result<User, DomainError>.Success(user);
    }

    /// <summary>
    /// Sets the email verification token.
    /// </summary>
    public void SetEmailVerificationToken(string token, TimeSpan expirationDuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        EmailVerificationToken = token;
        EmailVerificationTokenExpiresAt = DateTime.UtcNow.Add(expirationDuration);
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Verifies the user's email.
    /// </summary>
    public Result<DomainError> VerifyEmail(string token)
    {
        if (IsEmailVerified)
        {
            return Result<DomainError>.Failure(UserErrors.EmailAlreadyVerified);
        }

        if (string.IsNullOrWhiteSpace(EmailVerificationToken) ||
            !EmailVerificationToken.Equals(token, StringComparison.Ordinal))
        {
            return Result<DomainError>.Failure(UserErrors.InvalidEmailVerificationToken);
        }

        if (EmailVerificationTokenExpiresAt is null ||
            DateTime.UtcNow > EmailVerificationTokenExpiresAt)
        {
            return Result<DomainError>.Failure(UserErrors.InvalidEmailVerificationToken);
        }

        IsEmailVerified = true;
        EmailVerificationToken = null;
        EmailVerificationTokenExpiresAt = null;
        UpdatedAt = DateTime.UtcNow;

        Raise(new UserEmailVerifiedEvent(Id, Email.Value));

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Validates if the user can log in.
    /// </summary>
    public Result<DomainError> ValidateCanLogin()
    {
        if (Status == UserStatus.Deleted)
        {
            return Result<DomainError>.Failure(UserErrors.AccountDeleted);
        }

        if (Status == UserStatus.Suspended)
        {
            return Result<DomainError>.Failure(UserErrors.AccountSuspended);
        }

        if (!IsEmailVerified)
        {
            return Result<DomainError>.Failure(UserErrors.EmailNotVerified);
        }

        if (IsAccountLocked())
        {
            return Result<DomainError>.Failure(UserErrors.AccountLocked);
        }

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Records a successful login.
    /// </summary>
    public void RecordSuccessfulLogin(Guid sessionId, string ipAddress)
    {
        FailedLoginAttempts = 0;
        LockedUntil = null;
        LastLoginAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;

        Raise(new UserLoggedInEvent(Id, sessionId, ipAddress));
    }

    /// <summary>
    /// Records a failed login attempt.
    /// </summary>
    public void RecordFailedLogin(string ipAddress)
    {
        FailedLoginAttempts++;
        UpdatedAt = DateTime.UtcNow;

        Raise(new LoginFailedEvent(Id, Email.Value, ipAddress, FailedLoginAttempts));

        if (FailedLoginAttempts >= MaxFailedLoginAttempts)
        {
            LockAccount();
        }
    }

    /// <summary>
    /// Locks the account.
    /// </summary>
    private void LockAccount()
    {
        LockedUntil = DateTime.UtcNow.Add(AccountLockoutDuration);
        UpdatedAt = DateTime.UtcNow;

        Raise(new AccountLockedEvent(Id, LockedUntil.Value, FailedLoginAttempts));
    }

    /// <summary>
    /// Checks if the account is currently locked.
    /// </summary>
    public bool IsAccountLocked()
    {
        if (LockedUntil is null)
        {
            return false;
        }

        // Auto-unlock if lockout period has passed
        if (DateTime.UtcNow > LockedUntil)
        {
            UnlockAccount();
            return false;
        }

        return true;
    }

    /// <summary>
    /// Unlocks the account.
    /// </summary>
    public void UnlockAccount()
    {
        FailedLoginAttempts = 0;
        LockedUntil = null;
        UpdatedAt = DateTime.UtcNow;

        Raise(new AccountUnlockedEvent(Id));
    }

    /// <summary>
    /// Sets the password reset token.
    /// </summary>
    public void SetPasswordResetToken(string token, TimeSpan expirationDuration)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(token);

        PasswordResetToken = token;
        PasswordResetTokenExpiresAt = DateTime.UtcNow.Add(expirationDuration);
        UpdatedAt = DateTime.UtcNow;

        Raise(new PasswordResetRequestedEvent(Id, Email.Value, token));
    }

    /// <summary>
    /// Changes the user's password.
    /// </summary>
    public Result<DomainError> ChangePassword(PasswordHash newPasswordHash)
    {
        // Check if password was recently used
        if (IsPasswordInHistory(newPasswordHash))
        {
            return Result<DomainError>.Failure(UserErrors.PasswordReused);
        }

        PasswordHash = newPasswordHash;
        PasswordResetToken = null;
        PasswordResetTokenExpiresAt = null;
        UpdatedAt = DateTime.UtcNow;

        // Add to password history
        _passwordHistory.Add(newPasswordHash);

        // Keep only the last N passwords
        while (_passwordHistory.Count > PasswordHistoryLimit)
        {
            _passwordHistory.RemoveAt(0);
        }

        Raise(new PasswordChangedEvent(Id));

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Checks if a password hash exists in the password history.
    /// </summary>
    private bool IsPasswordInHistory(PasswordHash passwordHash)
    {
        return _passwordHistory.Any(h => h.Value.Equals(passwordHash.Value, StringComparison.Ordinal));
    }

    /// <summary>
    /// Validates the password reset token.
    /// </summary>
    public Result<DomainError> ValidatePasswordResetToken(string token)
    {
        if (string.IsNullOrWhiteSpace(PasswordResetToken) ||
            !PasswordResetToken.Equals(token, StringComparison.Ordinal))
        {
            return Result<DomainError>.Failure(UserErrors.InvalidPasswordResetToken);
        }

        if (PasswordResetTokenExpiresAt is null ||
            DateTime.UtcNow > PasswordResetTokenExpiresAt)
        {
            return Result<DomainError>.Failure(UserErrors.PasswordResetTokenExpired);
        }

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Assigns a role to the user.
    /// </summary>
    public Result<DomainError> AssignRole(Guid roleId, string roleName)
    {
        if (_roleIds.Contains(roleId))
        {
            return Result<DomainError>.Failure(UserErrors.RoleAlreadyAssigned);
        }

        _roleIds.Add(roleId);
        UpdatedAt = DateTime.UtcNow;

        Raise(new RoleAssignedToUserEvent(Id, roleId, roleName));

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Revokes a role from the user.
    /// </summary>
    public Result<DomainError> RevokeRole(Guid roleId, string roleName)
    {
        if (!_roleIds.Contains(roleId))
        {
            return Result<DomainError>.Failure(UserErrors.RoleNotAssigned);
        }

        _roleIds.Remove(roleId);
        UpdatedAt = DateTime.UtcNow;

        Raise(new RoleRevokedFromUserEvent(Id, roleId, roleName));

        return Result<DomainError>.Success();
    }

    /// <summary>
    /// Checks if the user has a specific role.
    /// </summary>
    public bool HasRole(Guid roleId)
    {
        return _roleIds.Contains(roleId);
    }

    /// <summary>
    /// Suspends the user account.
    /// </summary>
    public void Suspend()
    {
        Status = UserStatus.Suspended;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the user account.
    /// </summary>
    public void Activate()
    {
        Status = UserStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Soft deletes the user account.
    /// </summary>
    public void Delete()
    {
        Status = UserStatus.Deleted;
        UpdatedAt = DateTime.UtcNow;
    }
}

