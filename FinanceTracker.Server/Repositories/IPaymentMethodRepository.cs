using FinanceTracker.Server.Models.Entities;

namespace FinanceTracker.Server.Repositories
{
    public interface IPaymentMethodRepository
    {
        Task<IEnumerable<PaymentMethod>> GetPaymentMethods();
        Task<bool> PaymentMethodExistsAsync(Guid paymentMethodId);
    }
}
