using FinanceTracker.Domain.Enums;
using FluentValidation;

namespace FinanceTracker.Application.Features.Transactions.Validators;

public class CreateTransactionDtoValidator : AbstractValidator<CreateTransactionDto>
{
    public CreateTransactionDtoValidator()
    {
        // Reimbursement Rules 
        // 'Reimbursement' details depends on the 'IsReimbursement' boolean.
        RuleFor(x => x.Reimbursement)
            .NotNull()
            .When(x => x.IsReimbursement) 
            .WithMessage("Reimbursement details must be provided if the transaction is marked as a reimbursement.");

        RuleFor(x => x.Reimbursement)
            .Null()
            .When(x => !x.IsReimbursement) 
            .WithMessage("Reimbursement details should not be provided for a non-reimbursement transaction.");

        // Credit Card Rules 
        // 'Installment' details depends on the 'IsCreditCardPurchase'
        RuleFor(x => x.Installment)
            .NotNull()
            .When(x => x.IsCreditCardPurchase)
            .WithMessage("Installment details must be provided if the transaction is marked as a credit card purchase.");

        RuleFor(x => x.Installment)
            .Null()
            .When(x => !x.IsCreditCardPurchase)
            .WithMessage("Installment details should not be provided for a non-credit card purchase.");


        // Expense Rules
        // This rules will only be executed if the transaction Type is 'Expense'

        When(x => x.Type == TransactionType.Expense, () =>
        {
            RuleFor(x => x.CategoryId)
                .NotEmpty()
                .WithMessage("CategoryId is required for expense transactions.");

            RuleFor(x => x.PaymentMethodId)
                .NotEmpty()
                .WithMessage("PaymentMethodId is required for expense transactions.");
        });
    }
}
