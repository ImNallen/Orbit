using FluentValidation;

namespace Application.Users.Commands.SwitchLocationContext;

/// <summary>
/// Validator for SwitchLocationContextCommand.
/// </summary>
public sealed class SwitchLocationContextCommandValidator : AbstractValidator<SwitchLocationContextCommand>
{
    public SwitchLocationContextCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.LocationId)
            .NotEmpty().WithMessage("Location ID is required");
    }
}

