using FluentValidation;

namespace Application.Users.Commands.AssignUserToLocation;

/// <summary>
/// Validator for AssignUserToLocationCommand.
/// </summary>
public sealed class AssignUserToLocationCommandValidator : AbstractValidator<AssignUserToLocationCommand>
{
    public AssignUserToLocationCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.LocationId)
            .NotEmpty().WithMessage("Location ID is required");
    }
}

