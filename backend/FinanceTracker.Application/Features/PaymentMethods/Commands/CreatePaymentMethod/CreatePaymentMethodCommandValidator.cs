using FinanceTracker.Application.Features.PaymentMethods.Commands.CreatePaymentMethod;
using FluentValidation;

namespace FinanceTracker.Application.Features.PaymentMethods.Validators;

public class CreatePaymentMethodCommandValidator : AbstractValidator<CreatePaymentMethodCommand>
{
    public CreatePaymentMethodCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The payment method name is required.")
            .Length(3, 50).WithMessage("The name must be between 3 and 50 characters.");

        RuleFor(x => x.Type)
            .NotEmpty().WithMessage("The payment method type is required.")
            .Length(3, 50).WithMessage("The type must be between 3 and 50 characters.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required.");
    }
}
