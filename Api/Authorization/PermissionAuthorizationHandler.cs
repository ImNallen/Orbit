using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api.Authorization;

/// <summary>
/// Authorization handler that validates if a user has the required permission claim.
/// </summary>
public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly ILogger<PermissionAuthorizationHandler> _logger;

    public PermissionAuthorizationHandler(ILogger<PermissionAuthorizationHandler> logger)
    {
        _logger = logger;
    }

    /// <summary>
    /// Makes a decision if authorization is allowed based on the permission requirement.
    /// </summary>
    /// <param name="context">The authorization context.</param>
    /// <param name="requirement">The permission requirement to evaluate.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        string? userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        string? email = context.User.FindFirst(ClaimTypes.Email)?.Value;

        // Get all permission claims from the user
        IEnumerable<string> permissions = context.User
            .FindAll(claim => claim.Type == "permission")
            .Select(claim => claim.Value);

        // Check if the user has the required permission
        if (permissions.Contains(requirement.Permission))
        {
            _logger.LogDebug("Authorization succeeded for user {UserId} ({Email}): Has permission {Permission}",
                userId, email, requirement.Permission);
            context.Succeed(requirement);
        }
        else
        {
            _logger.LogWarning("Authorization failed for user {UserId} ({Email}): Missing permission {Permission}",
                userId, email, requirement.Permission);
        }

        return Task.CompletedTask;
    }
}

