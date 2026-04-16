using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Application.Features.PaymentMethods.Models;

public record PaymentMethodDto(
    Guid Id,
    string Name,
    string Type)
{
    public static PaymentMethodDto MapFrom(PaymentMethod paymentMethod) => new(
        paymentMethod.Id,
        paymentMethod.Name,
        paymentMethod.Type.ToString()
    );
}
