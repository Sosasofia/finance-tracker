using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.PaymentMethods.Models;

namespace FinanceTracker.Application.Features.PaymentMethods.Queries.GetPaymentMethods;

public record GetPaymentMethodsQuery(Guid UserId) : IRequest<IEnumerable<PaymentMethodDto>>;
