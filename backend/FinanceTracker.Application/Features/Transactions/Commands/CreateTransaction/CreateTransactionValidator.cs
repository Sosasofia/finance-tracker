using FinanceTracker.Application.Features.Transactions.Commands.CreateTransaction;
using FinanceTracker.Domain.Enums;
using FluentValidation;

namespace FinanceTracker.Application.Features.Transactions.Validators;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(100);

        RuleFor(x => x.Amount)
            .GreaterThan(0).WithMessage("Amount must be positive.");

        RuleFor(x => x.Date)
            .NotEmpty()
            .LessThanOrEqualTo(_ => DateTime.UtcNow.AddMinutes(5))
            .WithMessage("The transaction date cannot be in the future.");

        RuleFor(v => v.Description)
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters.");

        RuleFor(v => v.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes must not exceed 1000 characters.");

        When(x => x.IsReimbursement, () =>
        {
            RuleFor(x => x.Reimbursement)
                .NotNull().WithMessage("Reimbursement details must be provided.");
        }).Otherwise(() =>
        {
            RuleFor(x => x.Reimbursement)
                .Null().WithMessage("Reimbursement details should not be provided.");
        });

        When(x => x.Type == TransactionType.Expense, () =>
        {
            RuleFor(x => x.CategoryId).NotEmpty().WithMessage("Category is required for expenses.");
            RuleFor(x => x.PaymentMethodId).NotEmpty().WithMessage("Payment method is required for expenses.");
        });

        When(x => x.IsCreditCardPurchase, () =>
        {
            RuleFor(x => x.Installment)
                .NotNull().WithMessage("Installment details must be provided.")
                .SetValidator(new InstallmentDtoValidator()!); // Use the nested validator
        }).Otherwise(() =>
        {
            RuleFor(x => x.Installment)
                .Null().WithMessage("Installment details should not be provided.");
        });
    }
}
