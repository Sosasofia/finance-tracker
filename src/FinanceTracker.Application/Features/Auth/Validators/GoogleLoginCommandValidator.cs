using FinanceTracker.Application.Features.Auth.Commands.GoogleLogin;
using FluentValidation;

namespace FinanceTracker.Application.Features.Auth.Validators;

public class GoogleLoginCommandValidator : AbstractValidator<GoogleLoginCommand>
{
    public GoogleLoginCommandValidator()
    {
        RuleFor(x => x.IdToken)
            .NotEmpty().WithMessage("Google ID Token is required.")
            .Must(token => token.Split('.').Length == 3)
            .WithMessage("Invalid Google ID Token format.");
    }
}
