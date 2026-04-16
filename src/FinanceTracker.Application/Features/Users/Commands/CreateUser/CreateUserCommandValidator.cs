using FluentValidation;

namespace FinanceTracker.Application.Features.Users.Commands.CreateUser;
public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
{
    public CreateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");

        RuleFor(x => x.Name)
            .MaximumLength(100).WithMessage("Name cannot exceed 100 characters.")
            .MinimumLength(2).When(x => !string.IsNullOrEmpty(x.Name))
            .WithMessage("Name must be at least 2 characters long.");
    }
}
