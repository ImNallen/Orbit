using System.Security.Claims;
using Api.GraphQL.Payloads;
using Api.GraphQL.Types;
using Application.Users.Queries.GetCurrentUser;
using Application.Users.Queries.GetUserById;
using Application.Users.Queries.GetUsers;
using Application.Users.Queries.GetUserSessions;
using Domain.Abstractions;
using HotChocolate.Authorization;
using MediatR;

namespace Api.GraphQL.Queries;

/// <summary>
/// GraphQL queries for users.
/// </summary>
[ExtendObjectType("Query")]
public sealed class UserQueries
{
    /// <summary>
    /// Get the current authenticated user.
    /// Requires authentication.
    /// </summary>
    [Authorize]
    public async Task<UserType?> MeAsync(
        ClaimsPrincipal claimsPrincipal,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        // Get user ID from claims
        string? userIdString = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
        {
            return null;
        }

        var query = new GetCurrentUserQuery(userId);
        Result<GetCurrentUserResult, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return null;
        }

        return new UserType
        {
            UserId = result.Value.UserId,
            Email = result.Value.Email,
            FirstName = result.Value.FirstName,
            LastName = result.Value.LastName,
            IsEmailVerified = result.Value.IsEmailVerified,
            Status = result.Value.Status,
            CreatedAt = result.Value.CreatedAt,
            LastLoginAt = result.Value.LastLoginAt,
            Role = result.Value.Role,
            Permissions = result.Value.Permissions
        };
    }

    /// <summary>
    /// Get all users with pagination.
    /// Requires users:read permission.
    /// </summary>
    [Authorize(Policy = "users:read")]
    public async Task<UsersPayload> UsersAsync(
        [Service] IMediator mediator,
        int page = 1,
        int pageSize = 10,
        CancellationToken cancellationToken = default)
    {
        var query = new GetUsersQuery(page, pageSize);
        Result<GetUsersResult, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return new UsersPayload
            {
                Users = Array.Empty<UserSummaryType>(),
                TotalCount = 0,
                Page = page,
                PageSize = pageSize,
                TotalPages = 0,
                Errors = new[] { new UserError(result.Error.Code, result.Error.Message) }
            };
        }

        return new UsersPayload
        {
            Users = result.Value.Users.Select(u => new UserSummaryType
            {
                UserId = u.UserId,
                Email = u.Email,
                FirstName = u.FirstName,
                LastName = u.LastName,
                IsEmailVerified = u.IsEmailVerified,
                Status = u.Status,
                CreatedAt = u.CreatedAt,
                LastLoginAt = u.LastLoginAt,
                Role = u.RoleName
            }).ToList(),
            TotalCount = result.Value.TotalCount,
            Page = result.Value.Page,
            PageSize = result.Value.PageSize,
            TotalPages = result.Value.TotalPages,
            Errors = Array.Empty<UserError>()
        };
    }

    /// <summary>
    /// Get a user by their ID.
    /// Requires users:read permission.
    /// </summary>
    [Authorize(Policy = "users:read")]
    public async Task<UserSummaryType?> UserAsync(
        Guid id,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        var query = new GetUserByIdQuery(id);
        Result<GetUserByIdResult, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return null;
        }

        return new UserSummaryType
        {
            UserId = result.Value.UserId,
            Email = result.Value.Email,
            FirstName = result.Value.FirstName,
            LastName = result.Value.LastName,
            IsEmailVerified = result.Value.IsEmailVerified,
            Status = result.Value.Status,
            CreatedAt = result.Value.CreatedAt,
            LastLoginAt = result.Value.LastLoginAt
        };
    }

    /// <summary>
    /// Get all active sessions for the current user.
    /// Requires authentication.
    /// </summary>
    [Authorize]
    public async Task<SessionsPayload> SessionsAsync(
        ClaimsPrincipal claimsPrincipal,
        [Service] IMediator mediator,
        CancellationToken cancellationToken)
    {
        // Get user ID from claims
        string? userIdString = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out Guid userId))
        {
            return SessionsPayload.Failure(new UserError("Auth.Unauthorized", "User not authenticated"));
        }

        var query = new GetUserSessionsQuery(userId);
        Result<GetUserSessionsResult, DomainError> result = await mediator.Send(query, cancellationToken);

        if (result.IsFailure)
        {
            return SessionsPayload.Failure(new UserError(result.Error.Code, result.Error.Message));
        }

        // Map to GraphQL types
        var sessions = result.Value.Sessions.Select(s => new SessionType
        {
            SessionId = s.SessionId,
            IpAddress = s.IpAddress,
            UserAgent = s.UserAgent,
            CreatedAt = s.CreatedAt,
            ExpiresAt = s.ExpiresAt,
            LastAccessedAt = s.LastAccessedAt,
            IsCurrentSession = s.IsCurrentSession
        }).ToList();

        return SessionsPayload.Success(sessions);
    }
}
