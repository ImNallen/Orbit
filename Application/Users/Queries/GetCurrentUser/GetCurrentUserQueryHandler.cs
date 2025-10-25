using Domain.Abstractions;
using Domain.Users.Errors;
using Domain.Users.User;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Queries.GetCurrentUser;

/// <summary>
/// Handler for GetCurrentUserQuery.
/// </summary>
public sealed class GetCurrentUserQueryHandler
    : IRequestHandler<GetCurrentUserQuery, Result<GetCurrentUserResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<GetCurrentUserQueryHandler> _logger;

    public GetCurrentUserQueryHandler(
        IUserRepository userRepository,
        IAuthorizationService authorizationService,
        ILogger<GetCurrentUserQueryHandler> logger)
    {
        _userRepository = userRepository;
        _authorizationService = authorizationService;
        _logger = logger;
    }

    public async Task<Result<GetCurrentUserResult, DomainError>> Handle(
        GetCurrentUserQuery query,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting current user {UserId}", query.UserId);

        // 1. Get user by ID
        User? user = await _userRepository.GetByIdAsync(query.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", query.UserId);
            return Result<GetCurrentUserResult, DomainError>.Failure(UserErrors.UserNotFound);
        }

        // 2. Load user roles and permissions
        List<string> roles = await _authorizationService.GetUserRolesAsync(user.Id, cancellationToken);
        List<string> permissions = await _authorizationService.GetUserPermissionsAsync(user.Id, cancellationToken);

        _logger.LogDebug("User {UserId} has {RoleCount} roles and {PermissionCount} permissions",
            user.Id, roles.Count, permissions.Count);

        // 3. Return result
        return Result<GetCurrentUserResult, DomainError>.Success(
            new GetCurrentUserResult(
                user.Id,
                user.Email.Value,
                user.FullName.FirstName,
                user.FullName.LastName,
                user.IsEmailVerified,
                user.Status.ToString(),
                user.CreatedAt,
                user.LastLoginAt,
                roles,
                permissions));
    }
}

