using FinanceTracker.Application.Features.PaymentMethods;

namespace FinanceTracker.Application.Common.Interfaces.Services;

public interface IPaymentMethodService
{
    Task<IEnumerable<PaymentMethodDto>> GetPaymentMethodsAsync();
}
