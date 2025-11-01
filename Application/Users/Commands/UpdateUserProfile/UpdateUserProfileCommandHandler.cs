using Domain.Abstractions;
using Domain.Shared.ValueObjects;
using Domain.Users;
using Domain.Users.ValueObjects;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.UpdateUserProfile;

/// <summary>
/// Handler for UpdateUserProfileCommand.
/// </summary>
public sealed class UpdateUserProfileCommandHandler
    : IRequestHandler<UpdateUserProfileCommand, Result<UpdateUserProfileResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<UpdateUserProfileCommandHandler> _logger;

    public UpdateUserProfileCommandHandler(
        IUserRepository userRepository,
        ILogger<UpdateUserProfileCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<UpdateUserProfileResult, DomainError>> Handle(
        UpdateUserProfileCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Updating profile for user {UserId}",
            command.UserId);

        // 1. Get user
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            return Result<UpdateUserProfileResult, DomainError>.Failure(UserErrors.UserNotFound);
        }

        // 2. Create FullName value object
        Result<FullName, DomainError> fullNameResult = FullName.Create(
            command.FirstName,
            command.LastName);

        if (fullNameResult.IsFailure)
        {
            return Result<UpdateUserProfileResult, DomainError>.Failure(fullNameResult.Error);
        }

        // 3. Update profile
        user.UpdateProfile(fullNameResult.Value);

        // 4. Save changes
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation(
            "Profile updated successfully for user {UserId}",
            command.UserId);

        return Result<UpdateUserProfileResult, DomainError>.Success(
            new UpdateUserProfileResult(
                user.Id,
                user.FullName.FirstName,
                user.FullName.LastName,
                "Profile updated successfully"));
    }
}

