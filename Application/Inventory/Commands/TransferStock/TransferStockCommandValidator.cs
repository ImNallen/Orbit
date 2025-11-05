using FluentValidation;

namespace Application.Inventory.Commands.TransferStock;

/// <summary>
/// Validator for TransferStockCommand.
/// </summary>
public sealed class TransferStockCommandValidator : AbstractValidator<TransferStockCommand>
{
    public TransferStockCommandValidator()
    {
        RuleFor(x => x.FromInventoryId)
            .NotEmpty().WithMessage("Source inventory ID is required");

        RuleFor(x => x.ToInventoryId)
            .NotEmpty().WithMessage("Destination inventory ID is required");

        RuleFor(x => x.Quantity)
            .GreaterThan(0).WithMessage("Quantity must be greater than zero");

        RuleFor(x => x.ToInventoryId)
            .NotEqual(x => x.FromInventoryId)
            .WithMessage("Cannot transfer stock to the same inventory location");

        RuleFor(x => x.Reason)
            .NotEmpty().WithMessage("Reason is required")
            .MaximumLength(500).WithMessage("Reason must not exceed 500 characters");
    }
}

