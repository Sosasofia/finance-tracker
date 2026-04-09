using AutoMapper;
using FinanceTracker.Application.Features.PaymentMethods.Models;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.PaymentMethods.Queries.GetPaymentMethods;

public class GetPaymentMethodsQueryHandler
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IMapper _mapper;

    public GetPaymentMethodsQueryHandler(IPaymentMethodRepository repository, IMapper mapper)
    {
        _paymentMethodRepository = repository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentMethodDto>> Handle(GetPaymentMethodsQuery query, CancellationToken ct)
    {
        var paymentMethods = await _paymentMethodRepository.GetPaymentMethods();

        return _mapper.Map<IEnumerable<PaymentMethodDto>>(paymentMethods);
    }
}
