using System.ComponentModel.DataAnnotations;

namespace FinanceTracker.Application.Features.PaymentMethods.Commands.CreatePaymentMethod;

public class CreatePaymentMethodCommand
{
    [Required]
    public required string Name { get; set; }

    [Required]
    public required string Type { get; set; }

    public Guid UserId { get; set; } 
}
