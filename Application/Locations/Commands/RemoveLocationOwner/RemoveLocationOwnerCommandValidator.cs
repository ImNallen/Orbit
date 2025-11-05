using FluentValidation;

namespace Application.Locations.Commands.RemoveLocationOwner;

/// <summary>
/// Validator for RemoveLocationOwnerCommand.
/// </summary>
public sealed class RemoveLocationOwnerCommandValidator : AbstractValidator<RemoveLocationOwnerCommand>
{
    public RemoveLocationOwnerCommandValidator()
    {
        RuleFor(x => x.LocationId)
            .NotEmpty()
            .WithMessage("Location ID is required.");
    }
}

