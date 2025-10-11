using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Repositories;

public interface IPaymentMethodRepository
{
    Task<IEnumerable<PaymentMethod>> GetPaymentMethods();
    Task<bool> PaymentMethodExistsAsync(Guid paymentMethodId);
}
