using FluentValidation;
using FinanceTracker.Application.Features.Transactions.Models;

namespace FinanceTracker.Application.Features.Transactions.Validators;

public class InstallmentDtoValidator : AbstractValidator<InstallmentDto>
{
    public InstallmentDtoValidator()
    {
        RuleFor(x => x.Number)
            .InclusiveBetween(1, 12)
            .WithMessage("Number of installments must be between 1 and 12.");

        RuleFor(x => x.Interest)
            .GreaterThanOrEqualTo(0)
            .When(x => x.Interest.HasValue)
            .WithMessage("Interest cannot be negative.");
    }
}
