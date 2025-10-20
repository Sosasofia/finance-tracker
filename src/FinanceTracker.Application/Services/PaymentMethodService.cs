using AutoMapper;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.PaymentMethods;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Services;

public class PaymentMethodService : IPaymentMethodService
{
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IMapper _mapper;

    public PaymentMethodService(IPaymentMethodRepository paymentMethodRepository, IMapper mapper)
    {
        _paymentMethodRepository = paymentMethodRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<PaymentMethodDto>> GetPaymentMethodsAsync()
    {
        try
        {
            var paymentMethods = await _paymentMethodRepository.GetPaymentMethods();

            return _mapper.Map<IEnumerable<PaymentMethodDto>>(paymentMethods);
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message);
        }
    }
}
