using FluentValidation;

namespace Application.Users.Commands.SetPrimaryLocation;

/// <summary>
/// Validator for SetPrimaryLocationCommand.
/// </summary>
public sealed class SetPrimaryLocationCommandValidator : AbstractValidator<SetPrimaryLocationCommand>
{
    public SetPrimaryLocationCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.LocationId)
            .NotEmpty().WithMessage("Location ID is required");
    }
}

