using Domain.Abstractions;

namespace Domain.Users.Errors;

/// <summary>
/// Contains all user-related domain errors.
/// </summary>
public static class UserErrors
{
    // Authentication Errors
    public static readonly DomainError InvalidCredentials = new UserError(
        "User.InvalidCredentials",
        "The provided credentials are invalid.");

    public static readonly DomainError EmailNotVerified = new UserError(
        "User.EmailNotVerified",
        "Email address must be verified before logging in.");

    public static readonly DomainError AccountLocked = new UserError(
        "User.AccountLocked",
        "Account is locked due to multiple failed login attempts.");

    public static readonly DomainError AccountSuspended = new UserError(
        "User.AccountSuspended",
        "Account has been suspended.");

    public static readonly DomainError AccountDeleted = new UserError(
        "User.AccountDeleted",
        "Account has been deleted.");

    // Registration Errors
    public static readonly DomainError EmailAlreadyExists = new UserError(
        "User.EmailAlreadyExists",
        "A user with this email address already exists.");

    public static readonly DomainError InvalidEmail = new UserError(
        "User.InvalidEmail",
        "The provided email address is invalid.");

    public static readonly DomainError WeakPassword = new UserError(
        "User.WeakPassword",
        "Password does not meet security requirements.");

    public static readonly DomainError PasswordTooShort = new UserError(
        "User.PasswordTooShort",
        "Password must be at least 8 characters long.");

    public static readonly DomainError PasswordRequiresUppercase = new UserError(
        "User.PasswordRequiresUppercase",
        "Password must contain at least one uppercase letter.");

    public static readonly DomainError PasswordRequiresLowercase = new UserError(
        "User.PasswordRequiresLowercase",
        "Password must contain at least one lowercase letter.");

    public static readonly DomainError PasswordRequiresDigit = new UserError(
        "User.PasswordRequiresDigit",
        "Password must contain at least one digit.");

    public static readonly DomainError PasswordRequiresSpecialChar = new UserError(
        "User.PasswordRequiresSpecialChar",
        "Password must contain at least one special character.");

    public static readonly DomainError PasswordReused = new UserError(
        "User.PasswordReused",
        "Password has been used recently and cannot be reused.");

    // User Management Errors
    public static readonly DomainError UserNotFound = new UserError(
        "User.NotFound",
        "User not found.");

    public static readonly DomainError InvalidFirstName = new UserError(
        "User.InvalidFirstName",
        "First name is required and cannot be empty.");

    public static readonly DomainError InvalidLastName = new UserError(
        "User.InvalidLastName",
        "Last name is required and cannot be empty.");

    public static readonly DomainError NameTooLong = new UserError(
        "User.NameTooLong",
        "Name cannot exceed 100 characters.");

    // Password Reset Errors
    public static readonly DomainError InvalidPasswordResetToken = new UserError(
        "User.InvalidPasswordResetToken",
        "Password reset token is invalid or has expired.");

    public static readonly DomainError PasswordResetTokenExpired = new UserError(
        "User.PasswordResetTokenExpired",
        "Password reset token has expired.");

    // Email Verification Errors
    public static readonly DomainError InvalidEmailVerificationToken = new UserError(
        "User.InvalidEmailVerificationToken",
        "Email verification token is invalid or has expired.");

    public static readonly DomainError EmailAlreadyVerified = new UserError(
        "User.EmailAlreadyVerified",
        "Email address is already verified.");

    // Session Errors
    public static readonly DomainError SessionNotFound = new UserError(
        "User.SessionNotFound",
        "Session not found.");

    public static readonly DomainError SessionExpired = new UserError(
        "User.SessionExpired",
        "Session has expired.");

    public static readonly DomainError InvalidRefreshToken = new UserError(
        "User.InvalidRefreshToken",
        "Refresh token is invalid or has expired.");

    // Role and Permission Errors
    public static readonly DomainError RoleNotFound = new UserError(
        "User.RoleNotFound",
        "Role not found.");

    public static readonly DomainError PermissionNotFound = new UserError(
        "User.PermissionNotFound",
        "Permission not found.");

    public static readonly DomainError RoleAlreadyAssigned = new UserError(
        "User.RoleAlreadyAssigned",
        "Role is already assigned to this user.");

    public static readonly DomainError RoleNotAssigned = new UserError(
        "User.RoleNotAssigned",
        "Role is not assigned to this user.");

    public static readonly DomainError InsufficientPermissions = new UserError(
        "User.InsufficientPermissions",
        "User does not have the required permissions.");

    // Private error record
    private sealed record UserError(string Code, string Message) : DomainError(Code, Message);
}

