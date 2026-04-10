using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface IPaymentMethodRepository
{
    Task<IEnumerable<PaymentMethod>> GetPaymentMethods(Guid userId);
    Task<bool> ExistsAsync(Guid paymentMethodId);
    Task<bool> ExistsForUserAsync(Guid userId, string name);
    Task<PaymentMethod> AddAsync(PaymentMethod paymentMethod);
    Task<PaymentMethod?> GetByIdAsync(Guid paymentMethodId, Guid userId);
    Task<bool> IsInUseAsync(Guid paymentMethodId);
    Task DeleteAsync(PaymentMethod paymentMethod);
}
