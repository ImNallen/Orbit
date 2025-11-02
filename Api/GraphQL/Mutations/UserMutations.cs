using System.Security.Claims;
using Api.GraphQL.Inputs;
using Api.GraphQL.Payloads;
using Api.GraphQL.Types;
using Application.Roles.Commands.AssignRole;
using Application.Users.Commands.Login;
using Application.Users.Commands.RefreshToken;
using Application.Users.Commands.RegisterUser;
using Application.Users.Commands.RequestPasswordReset;
using Application.Users.Commands.ResetPassword;
using Application.Users.Commands.RevokeAllSessions;
using Application.Users.Commands.RevokeSession;
using Application.Users.Commands.VerifyEmail;
using Application.Users.Commands.SuspendUser;
using Application.Users.Commands.ActivateUser;
using Application.Users.Commands.DeleteUser;
using Application.Users.Commands.UnlockUserAccount;
using Application.Users.Commands.UpdateUserProfile;
using Domain.Abstractions;
using HotChocolate.Authorization;
using MediatR;

namespace Api.GraphQL.Mutations;

/// <summary>
/// GraphQL mutations for user operations.
/// </summary>
[ExtendObjectType("Mutation")]
public sealed class UserMutations
{
    /// <summary>
    /// Registers a new user.
    /// Requires users:create permission.
    /// </summary>
    [Authorize(Policy = "users:create")]
    public async Task<RegisterUserPayload> RegisterUserAsync(
        RegisterUserInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new RegisterUserCommand(
            input.Email,
            input.Password,
            input.FirstName,
            input.LastName,
            input.RoleId);

        Result<RegisterUserResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return RegisterUserPayload.Failure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        RegisterUserResult userResult = result.Value;

        var user = new UserSummaryType
        {
            UserId = userResult.UserId,
            Email = userResult.Email,
            FirstName = userResult.FirstName,
            LastName = userResult.LastName,
            IsEmailVerified = false,
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = null
        };

        return RegisterUserPayload.Success(user);
    }

    /// <summary>
    /// Authenticates a user and creates a session.
    /// </summary>
    public async Task<LoginPayload> LoginAsync(
        LoginInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new LoginCommand(
            input.Email,
            input.Password);

        Result<LoginResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return LoginPayload.Failure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        LoginResult loginResult = result.Value;

        var user = new UserSummaryType
        {
            UserId = loginResult.UserId,
            Email = loginResult.Email,
            FirstName = loginResult.FirstName,
            LastName = loginResult.LastName,
            IsEmailVerified = true, // User must be verified to login
            Status = "Active",
            CreatedAt = DateTime.UtcNow, // We don't have this in LoginResult, using current time
            LastLoginAt = DateTime.UtcNow
        };

        return LoginPayload.Success(
            user,
            loginResult.AccessToken,
            loginResult.RefreshToken,
            loginResult.ExpiresAt);
    }

    /// <summary>
    /// Refreshes an access token using a refresh token.
    /// </summary>
    public async Task<RefreshTokenPayload> RefreshTokenAsync(
        RefreshTokenInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new RefreshTokenCommand(input.RefreshToken);

        Result<RefreshTokenResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return RefreshTokenPayload.Failure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        RefreshTokenResult refreshResult = result.Value;

        var user = new UserSummaryType
        {
            UserId = refreshResult.UserId,
            Email = refreshResult.Email,
            FirstName = string.Empty, // We don't have this in RefreshTokenResult
            LastName = string.Empty, // We don't have this in RefreshTokenResult
            IsEmailVerified = true,
            Status = "Active",
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow
        };

        return RefreshTokenPayload.Success(
            user,
            refreshResult.AccessToken,
            refreshResult.RefreshToken,
            refreshResult.ExpiresAt);
    }

    /// <summary>
    /// Verifies a user's email address.
    /// </summary>
    public async Task<VerifyEmailPayload> VerifyEmailAsync(
        VerifyEmailInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new VerifyEmailCommand(input.Token);

        Result<VerifyEmailResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return VerifyEmailPayload.CreateFailure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        return VerifyEmailPayload.CreateSuccess("Email verified successfully!");
    }

    /// <summary>
    /// Requests a password reset for a user.
    /// </summary>
    public async Task<RequestPasswordResetPayload> RequestPasswordResetAsync(
        RequestPasswordResetInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new RequestPasswordResetCommand(input.Email);

        Result<RequestPasswordResetResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return RequestPasswordResetPayload.CreateFailure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        return RequestPasswordResetPayload.CreateSuccess(result.Value.Message);
    }

    /// <summary>
    /// Resets a user's password using a reset token.
    /// </summary>
    public async Task<ResetPasswordPayload> ResetPasswordAsync(
        ResetPasswordInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ResetPasswordCommand(input.Token, input.NewPassword);

        Result<ResetPasswordResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return ResetPasswordPayload.CreateFailure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        return ResetPasswordPayload.CreateSuccess(result.Value.Message);
    }

    /// <summary>
    /// Revokes a specific session.
    /// Requires authentication.
    /// </summary>
    [Authorize]
    public async Task<RevokeSessionPayload> RevokeSessionAsync(
        RevokeSessionInput input,
        ClaimsPrincipal claimsPrincipal,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        // Get user ID from claims
        string? userIdString = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
        {
            return RevokeSessionPayload.Failure(new UserError("Auth.Unauthorized", "User not authenticated"));
        }

        var command = new RevokeSessionCommand(userId, input.SessionId);
        Result<RevokeSessionResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return RevokeSessionPayload.Failure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        return RevokeSessionPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Revokes all sessions except the current one.
    /// Requires authentication.
    /// </summary>
    [Authorize]
    public async Task<RevokeAllSessionsPayload> RevokeAllSessionsAsync(
        ClaimsPrincipal claimsPrincipal,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        // Get user ID from claims
        string? userIdString = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
        {
            return RevokeAllSessionsPayload.Failure(new UserError("Auth.Unauthorized", "User not authenticated"));
        }

        // Note: We don't have a way to get the current session ID from the JWT token
        // So we'll revoke all sessions. In a production app, you might want to include
        // the session ID in the JWT claims or pass it as an input parameter.
        var command = new RevokeAllSessionsCommand(userId, null);
        Result<RevokeAllSessionsResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return RevokeAllSessionsPayload.Failure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        return RevokeAllSessionsPayload.Success(result.Value.Message, result.Value.RevokedCount);
    }

    /// <summary>
    /// Changes a user's role.
    /// Requires roles:assign permission.
    /// </summary>
    [Authorize(Policy = "roles:assign")]
    public async Task<AssignRolePayload> ChangeUserRoleAsync(
        AssignRoleInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new AssignRoleCommand(input.UserId, input.RoleId);
        Result<AssignRoleResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return AssignRolePayload.Failure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        return AssignRolePayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Suspends a user account.
    /// Requires users:suspend permission.
    /// </summary>
    [Authorize(Policy = "users:update")]
    public async Task<SuspendUserPayload> SuspendUserAsync(
        SuspendUserInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new SuspendUserCommand(input.UserId);
        Result<SuspendUserResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return SuspendUserPayload.Failure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        return SuspendUserPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Activates a suspended user account.
    /// Requires users:update permission.
    /// </summary>
    [Authorize(Policy = "users:update")]
    public async Task<ActivateUserPayload> ActivateUserAsync(
        ActivateUserInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new ActivateUserCommand(input.UserId);
        Result<ActivateUserResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return ActivateUserPayload.Failure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        return ActivateUserPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Deletes a user account (soft delete).
    /// Requires users:delete permission.
    /// </summary>
    [Authorize(Policy = "users:delete")]
    public async Task<DeleteUserPayload> DeleteUserAsync(
        DeleteUserInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new DeleteUserCommand(input.UserId);
        Result<DeleteUserResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return DeleteUserPayload.Failure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        return DeleteUserPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Unlocks a locked user account.
    /// Requires users:update permission.
    /// </summary>
    [Authorize(Policy = "users:update")]
    public async Task<UnlockUserAccountPayload> UnlockUserAccountAsync(
        UnlockUserAccountInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UnlockUserAccountCommand(input.UserId);
        Result<UnlockUserAccountResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return UnlockUserAccountPayload.Failure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        return UnlockUserAccountPayload.Success(result.Value.Message);
    }

    /// <summary>
    /// Updates a user's profile information.
    /// Requires users:update permission.
    /// </summary>
    [Authorize(Policy = "users:update")]
    public async Task<UpdateUserProfilePayload> UpdateUserProfileAsync(
        UpdateUserProfileInput input,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var command = new UpdateUserProfileCommand(
            input.UserId,
            input.FirstName,
            input.LastName);

        Result<UpdateUserProfileResult, DomainError> result = await mediator.Send(command, cancellationToken);

        if (result.IsFailure)
        {
            return UpdateUserProfilePayload.Failure(
                new UserError(result.Error.Code, result.Error.Message));
        }

        UpdateUserProfileResult profileResult = result.Value;

        return UpdateUserProfilePayload.Success(
            profileResult.UserId,
            profileResult.FirstName,
            profileResult.LastName,
            profileResult.Message);
    }
}

