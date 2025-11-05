using FluentValidation;

namespace Application.Locations.Commands.AssignLocationManager;

/// <summary>
/// Validator for AssignLocationManagerCommand.
/// </summary>
public sealed class AssignLocationManagerCommandValidator : AbstractValidator<AssignLocationManagerCommand>
{
    public AssignLocationManagerCommandValidator()
    {
        RuleFor(x => x.LocationId)
            .NotEmpty()
            .WithMessage("Location ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}

