using FinanceTracker.Application.Features.Transactions.Commands.CreateTransaction;
using FinanceTracker.Domain.Enums;
using FluentValidation;

namespace FinanceTracker.Application.Features.Transactions.Validators;

public class CreateTransactionCommandValidator : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
        RuleFor(x => x.Amount).GreaterThan(0);
        RuleFor(x => x.Date)
            .NotEmpty()
            .LessThanOrEqualTo(_ => DateTime.UtcNow.AddMinutes(5))
            .WithMessage("The transaction date cannot be in the future.");

        RuleSet("ReimbursementLogic", () => {
            When(x => x.IsReimbursement, () => {
                RuleFor(x => x.Reimbursement)
                    .NotNull()
                    .WithMessage("Reimbursement details must be provided.");
            }).Otherwise(() => {
                RuleFor(x => x.Reimbursement)
                    .Null()
                    .WithMessage("Reimbursement details should not be provided for non-reimbursement transactions.");
            });
        });

        RuleSet("CreditCardLogic", () => {
            When(x => x.IsCreditCardPurchase, () => {
                RuleFor(x => x.Installment)
                    .NotNull()
                    .WithMessage("Installment details must be provided.");
            }).Otherwise(() => {
                RuleFor(x => x.Installment)
                    .Null()
                    .WithMessage("Installment details should not be provided for non-credit card purchases.");
            });
        });

        When(x => x.Type == TransactionType.Expense, () =>
        {
            RuleFor(x => x.CategoryId).NotEmpty();
            RuleFor(x => x.PaymentMethodId).NotEmpty();
        });
    }
}
