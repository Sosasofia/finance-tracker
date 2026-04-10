using FinanceTracker.Application.Features.PaymentMethods.Models;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.PaymentMethods.Queries.GetPaymentMethods;

public class GetPaymentMethodsQueryHandler
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public GetPaymentMethodsQueryHandler(IPaymentMethodRepository repository)
    {
        _paymentMethodRepository = repository;
    }

    public async Task<IEnumerable<PaymentMethodDto>> Handle(GetPaymentMethodsQuery query, CancellationToken ct)
    {
        var paymentMethods = await _paymentMethodRepository.GetPaymentMethods(query.UserId);

        return paymentMethods.Select(PaymentMethodDto.MapFrom);
    }
}
