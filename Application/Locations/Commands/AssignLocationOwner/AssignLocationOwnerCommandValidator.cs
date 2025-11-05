using FluentValidation;

namespace Application.Locations.Commands.AssignLocationOwner;

/// <summary>
/// Validator for AssignLocationOwnerCommand.
/// </summary>
public sealed class AssignLocationOwnerCommandValidator : AbstractValidator<AssignLocationOwnerCommand>
{
    public AssignLocationOwnerCommandValidator()
    {
        RuleFor(x => x.LocationId)
            .NotEmpty()
            .WithMessage("Location ID is required.");

        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("User ID is required.");
    }
}

