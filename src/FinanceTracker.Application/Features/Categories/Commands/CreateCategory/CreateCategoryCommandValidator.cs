using FinanceTracker.Application.Features.Categories.Commands.CreateCategory;
using FluentValidation;

namespace FinanceTracker.Application.Features.Categories.Validators;

public class CreateCategoryCommandValidator : AbstractValidator<CreateCategoryCommand>
{
    public CreateCategoryCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("The category name is required.")
            .Length(3, 50).WithMessage("The name must be between 3 and 50 characters.");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("UserId is required to create a category.");
    }
}
