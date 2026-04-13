using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Features.PaymentMethods.Models;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Features.PaymentMethods.Queries.GetById;

public class GetPaymentMethodByIdQueryHandler : IRequestHandler<GetPaymentMethodByIdQuery, PaymentMethodDto>
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public GetPaymentMethodByIdQueryHandler(IPaymentMethodRepository repository)
    {
        _paymentMethodRepository = repository;
    }

    public async Task<PaymentMethodDto> Handle(GetPaymentMethodByIdQuery query, CancellationToken ct)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(query.Id, query.UserId, ct)
            ?? throw new NotFoundException(nameof(PaymentMethod), query.Id);

        return PaymentMethodDto.MapFrom(paymentMethod);
    }
}
