using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.Features.PaymentMethods.Commands.CreatePaymentMethod;

public record CreatePaymentMethodCommand(
    [Required(ErrorMessage = "The payment method name is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "The name must be between 3 and 50 characters.")] 
    string Name,
    [Required(ErrorMessage = "The payment method type is required.")]
    [StringLength(50, MinimumLength = 3, ErrorMessage = "The name must be between 3 and 50 characters.")]
    string Type
)
{
    public Guid UserId { get; init; }
}
