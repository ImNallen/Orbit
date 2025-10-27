using Application.Abstractions.Email;
using Domain.Abstractions;
using Domain.Users.Errors;
using Domain.Users.Role;
using Domain.Users.User;
using MediatR;

namespace Application.Users.Commands.RegisterUser;

/// <summary>
/// Handler for RegisterUserCommand.
/// </summary>
public sealed class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result<RegisterUserResult, DomainError>>
{
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly ITokenGenerator _tokenGenerator;
    private readonly IEmailService _emailService;

    public RegisterUserCommandHandler(
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IPasswordHasher passwordHasher,
        ITokenGenerator tokenGenerator,
        IEmailService emailService)
    {
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
        _emailService = emailService;
    }

    public async Task<Result<RegisterUserResult, DomainError>> Handle(
        RegisterUserCommand command,
        CancellationToken cancellationToken)
    {
        // 1. Create Email value object
        Result<Email, DomainError> emailResult = Email.Create(command.Email);
        if (emailResult.IsFailure)
        {
            return Result<RegisterUserResult, DomainError>.Failure(emailResult.Error);
        }

        Email email = emailResult.Value;

        // 2. Check if user already exists
        bool userExists = await _userRepository.ExistsByEmailAsync(email, cancellationToken);
        if (userExists)
        {
            return Result<RegisterUserResult, DomainError>.Failure(
                UserErrors.EmailAlreadyExists);
        }

        // 3. Create Password value object
        Result<Password, DomainError> passwordResult = Password.Create(command.Password);
        if (passwordResult.IsFailure)
        {
            return Result<RegisterUserResult, DomainError>.Failure(passwordResult.Error);
        }

        // 4. Hash the password
        PasswordHash passwordHash = _passwordHasher.Hash(passwordResult.Value);

        // 5. Create FullName value object
        Result<FullName, DomainError> fullNameResult = FullName.Create(command.FirstName, command.LastName);
        if (fullNameResult.IsFailure)
        {
            return Result<RegisterUserResult, DomainError>.Failure(fullNameResult.Error);
        }

        // 6. Determine role ID (use provided or default to "User" role)
        Guid roleId;
        if (command.RoleId.HasValue)
        {
            // Use the provided role ID
            roleId = command.RoleId.Value;

            // Verify the role exists
            Role? role = await _roleRepository.GetByIdAsync(roleId, cancellationToken);
            if (role is null)
            {
                return Result<RegisterUserResult, DomainError>.Failure(
                    UserErrors.RoleNotFound);
            }
        }
        else
        {
            // Default to "User" role
            Role? defaultRole = await _roleRepository.GetByNameAsync("User", cancellationToken);
            if (defaultRole is null)
            {
                return Result<RegisterUserResult, DomainError>.Failure(
                    UserErrors.RoleNotFound);
            }
            roleId = defaultRole.Id;
        }

        // 7. Create User entity with role
        Result<User, DomainError> userResult = User.Create(
            email,
            passwordHash,
            fullNameResult.Value,
            roleId);

        if (userResult.IsFailure)
        {
            return Result<RegisterUserResult, DomainError>.Failure(userResult.Error);
        }

        User user = userResult.Value;

        // 8. Generate email verification token
        string verificationToken = _tokenGenerator.GenerateToken();
        var tokenExpiration = TimeSpan.FromHours(24); // Token valid for 24 hours
        user.SetEmailVerificationToken(verificationToken, tokenExpiration);

        // 9. Save user to database
        await _userRepository.AddAsync(user, cancellationToken);

        // 10. Send verification email (fire and forget - don't block registration)
        _ = Task.Run(async () =>
        {
            try
            {
                await _emailService.SendEmailVerificationAsync(
                    user.Email.Value,
                    user.FullName.FirstName,
                    verificationToken,
                    CancellationToken.None);
            }
            catch
            {
                // Log error but don't fail registration
                // In production, you'd want proper logging here
            }
        }, cancellationToken);

        // 11. Return success result
        return Result<RegisterUserResult, DomainError>.Success(
            new RegisterUserResult(
                user.Id,
                user.Email.Value,
                user.FullName.FirstName,
                user.FullName.LastName));
    }
}
