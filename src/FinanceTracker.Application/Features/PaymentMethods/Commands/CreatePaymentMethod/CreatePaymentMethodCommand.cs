using FinanceTracker.Application.Features.PaymentMethods.Models;
using MediatR;

namespace FinanceTracker.Application.Features.PaymentMethods.Commands.CreatePaymentMethod;

public record CreatePaymentMethodCommand(
    string Name,
    string Type
) : IRequest<PaymentMethodDto>
{
    public Guid UserId { get; init; }
}
