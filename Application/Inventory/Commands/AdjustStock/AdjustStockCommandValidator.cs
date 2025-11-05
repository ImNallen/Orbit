using FluentValidation;

namespace Application.Inventory.Commands.AdjustStock;

/// <summary>
/// Validator for AdjustStockCommand.
/// </summary>
public sealed class AdjustStockCommandValidator : AbstractValidator<AdjustStockCommand>
{
    public AdjustStockCommandValidator()
    {
        RuleFor(x => x.InventoryId)
            .NotEmpty().WithMessage("Inventory ID is required");

        RuleFor(x => x.Adjustment)
            .NotEqual(0).WithMessage("Adjustment cannot be zero");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters");
    }
}

