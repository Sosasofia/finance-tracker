using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.PaymentMethods.Models;

namespace FinanceTracker.Application.Features.PaymentMethods.Queries.GetById;

public record GetPaymentMethodByIdQuery(Guid Id, Guid UserId) : IRequest<PaymentMethodDto>;
