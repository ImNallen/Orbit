using Domain.Abstractions;
using Domain.Users;
using Domain.Users.Enums;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Users.Commands.DeleteUser;

/// <summary>
/// Handler for DeleteUserCommand.
/// </summary>
public sealed class DeleteUserCommandHandler
    : IRequestHandler<DeleteUserCommand, Result<DeleteUserResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DeleteUserCommandHandler> _logger;

    public DeleteUserCommandHandler(
        IUserRepository userRepository,
        ILogger<DeleteUserCommandHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<Result<DeleteUserResult, DomainError>> Handle(
        DeleteUserCommand command,
        CancellationToken cancellationToken)
    {
        _logger.LogInformation("Deleting user {UserId}", command.UserId);

        // 1. Get user
        User? user = await _userRepository.GetByIdAsync(command.UserId, cancellationToken);
        if (user is null)
        {
            _logger.LogWarning("User {UserId} not found", command.UserId);
            return Result<DeleteUserResult, DomainError>.Failure(UserErrors.UserNotFound);
        }

        // 2. Check if already deleted
        if (user.Status == UserStatus.Deleted)
        {
            _logger.LogWarning("User {UserId} is already deleted", command.UserId);
            return Result<DeleteUserResult, DomainError>.Failure(UserErrors.AccountDeleted);
        }

        // 3. Soft delete user
        user.Delete();

        // 4. Save changes
        await _userRepository.UpdateAsync(user, cancellationToken);

        _logger.LogInformation("User {UserId} deleted successfully", command.UserId);

        return Result<DeleteUserResult, DomainError>.Success(
            new DeleteUserResult($"User {user.Email.Value} has been deleted"));
    }
}

