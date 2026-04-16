using FinanceTracker.Application.Features.Transactions.Models;
using FluentValidation;

namespace FinanceTracker.Application.Features.Transactions.Validators;

public class ReimbursementDtoValidator : AbstractValidator<ReimbursementDto>
{
    public ReimbursementDtoValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Reimbursement amount must be greater than zero.");

        RuleFor(x => x.Date)
            .NotEmpty()
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Date cannot be in the future.");

        RuleFor(x => x.Reason)
            .MaximumLength(200);
    }
}
