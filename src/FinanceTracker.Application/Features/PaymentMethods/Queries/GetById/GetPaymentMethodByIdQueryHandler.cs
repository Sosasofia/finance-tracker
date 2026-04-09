using AutoMapper;
using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Features.PaymentMethods.Models;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.PaymentMethods.Queries.GetById;

public class GetPaymentMethodByIdQueryHandler
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IMapper _mapper;

    public GetPaymentMethodByIdQueryHandler(IPaymentMethodRepository repository, IMapper mapper)
    {
        _paymentMethodRepository = repository;
        _mapper = mapper;
    }

    public async Task<PaymentMethodDto> Handle(GetPaymentMethodByIdQuery query, CancellationToken ct)
    {
        var paymentMethod = await _paymentMethodRepository.GetByIdAsync(query.Id)
            ?? throw new NotFoundException(nameof(PaymentMethod), query.Id);

        return _mapper.Map<PaymentMethodDto>(paymentMethod);
    }
}
