using FinanceTracker.Application.Features.PaymentMethods.Models;
using MediatR;

namespace FinanceTracker.Application.Features.PaymentMethods.Queries.GetById;

public record GetPaymentMethodByIdQuery(Guid Id, Guid UserId) : IRequest<PaymentMethodDto>;
