using Domain.Abstractions;
using Domain.Locations;
using Domain.UserLocations;
using Domain.UserLocations.Enums;
using Domain.Users;
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
    private readonly ILocationRepository _locationRepository;
    private readonly IAuthorizationService _authorizationService;
    private readonly ILogger<GetCurrentUserQueryHandler> _logger;

    public GetCurrentUserQueryHandler(
        IUserRepository userRepository,
        ILocationRepository locationRepository,
        IAuthorizationService authorizationService,
        ILogger<GetCurrentUserQueryHandler> logger)
    {
        _userRepository = userRepository;
        _locationRepository = locationRepository;
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

        // 2. Load user role and permissions
        string? role = await _authorizationService.GetUserRoleAsync(user.Id, cancellationToken);
        List<string> permissions = await _authorizationService.GetUserPermissionsAsync(user.Id, cancellationToken);

        _logger.LogDebug("User {UserId} has role {Role} and {PermissionCount} permissions",
            user.Id, role ?? "none", permissions.Count);

        // 3. Get user's active location assignments
        var activeAssignments = user.LocationAssignments
            .Where(a => a.Status == AssignmentStatus.Active)
            .ToList();

        // 4. Fetch location details for assigned locations
        var locationIds = activeAssignments.Select(a => a.LocationId).ToList();
        var assignedLocationDtos = new List<UserLocationDto>();

        foreach (Guid locationId in locationIds)
        {
            Location? location = await _locationRepository.GetByIdAsync(locationId, cancellationToken);
            if (location is not null)
            {
                UserLocationAssignment assignment = activeAssignments.First(a => a.LocationId == locationId);
                assignedLocationDtos.Add(new UserLocationDto(
                    location.Id,
                    location.Name.Value,
                    location.Type,
                    location.Status,
                    location.Address.City,
                    location.Address.State,
                    location.Address.Country,
                    assignment.IsPrimaryLocation,
                    user.CurrentLocationContextId == locationId));
            }
        }

        _logger.LogDebug("User {UserId} has {LocationCount} assigned locations",
            user.Id, assignedLocationDtos.Count);

        // 5. Return result
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
                role,
                permissions,
                user.CurrentLocationContextId,
                assignedLocationDtos));
    }
}

