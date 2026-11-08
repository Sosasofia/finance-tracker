using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface IPaymentMethodRepository
{
    Task<IEnumerable<PaymentMethod>> GetPaymentMethods(Guid userId, CancellationToken ct = default);
    Task<bool> ExistsAsync(Guid paymentMethodId, CancellationToken ct = default);
    Task<bool> ExistsForUserAsync(Guid userId, string name, CancellationToken ct = default);
    Task<PaymentMethod> AddAsync(PaymentMethod paymentMethod, CancellationToken ct = default);
    Task<PaymentMethod?> GetByIdAsync(Guid paymentMethodId, Guid userId, CancellationToken ct = default);
    Task<bool> IsInUseAsync(Guid paymentMethodId, CancellationToken ct = default);
    Task DeleteAsync(PaymentMethod paymentMethod, CancellationToken ct = default);
}
