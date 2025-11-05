using FluentValidation;

namespace Application.Users.Commands.UnassignUserFromLocation;

/// <summary>
/// Validator for UnassignUserFromLocationCommand.
/// </summary>
public sealed class UnassignUserFromLocationCommandValidator : AbstractValidator<UnassignUserFromLocationCommand>
{
    public UnassignUserFromLocationCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.LocationId)
            .NotEmpty().WithMessage("Location ID is required");
    }
}

