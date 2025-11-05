using FluentValidation;

namespace Application.Locations.Commands.RemoveLocationManager;

/// <summary>
/// Validator for RemoveLocationManagerCommand.
/// </summary>
public sealed class RemoveLocationManagerCommandValidator : AbstractValidator<RemoveLocationManagerCommand>
{
    public RemoveLocationManagerCommandValidator()
    {
        RuleFor(x => x.LocationId)
            .NotEmpty()
            .WithMessage("Location ID is required.");
    }
}

