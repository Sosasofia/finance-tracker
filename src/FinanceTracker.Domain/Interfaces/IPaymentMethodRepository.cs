using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface IPaymentMethodRepository
{
    Task<IEnumerable<PaymentMethod>> GetPaymentMethods();
    Task<bool> PaymentMethodExistsAsync(Guid paymentMethodId);
}
