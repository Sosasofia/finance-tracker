using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.PaymentMethods.Models;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.PaymentMethods.Queries.GetPaymentMethods;

public class GetPaymentMethodsQueryHandler : IRequestHandler<GetPaymentMethodsQuery, IEnumerable<PaymentMethodDto>>
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;

    public GetPaymentMethodsQueryHandler(IPaymentMethodRepository repository)
    {
        _paymentMethodRepository = repository;
    }

    public async Task<IEnumerable<PaymentMethodDto>> Handle(GetPaymentMethodsQuery query, CancellationToken ct)
    {
        var paymentMethods = await _paymentMethodRepository.GetPaymentMethods(query.UserId, ct);

        return paymentMethods.Select(PaymentMethodDto.MapFrom);
    }
}
