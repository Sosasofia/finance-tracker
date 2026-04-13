using FinanceTracker.Application.Features.PaymentMethods.Models;
using MediatR;

namespace FinanceTracker.Application.Features.PaymentMethods.Queries.GetPaymentMethods;

public record GetPaymentMethodsQuery(Guid UserId) : IRequest<IEnumerable<PaymentMethodDto>>;
