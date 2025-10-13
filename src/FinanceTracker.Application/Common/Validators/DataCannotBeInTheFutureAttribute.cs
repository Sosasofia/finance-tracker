using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.Common.Validators;

public class DataCannotBeInTheFutureAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if(value is DateTime date && date > DateTime.UtcNow)
        {
            return new ValidationResult("Date can not be in the future.");
        }

        return ValidationResult.Success;
    }
}
