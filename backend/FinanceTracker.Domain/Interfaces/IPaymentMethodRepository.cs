using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface IPaymentMethodRepository
{
    Task<IEnumerable<PaymentMethod>> GetPaymentMethods(Guid userId, CancellationToken ct);
    Task<bool> ExistsAsync(Guid paymentMethodId, CancellationToken ct);
    Task<bool> ExistsForUserAsync(Guid userId, string name, CancellationToken ct);
    Task<PaymentMethod> AddAsync(PaymentMethod paymentMethod, CancellationToken ct);
    Task<PaymentMethod?> GetByIdAsync(Guid paymentMethodId, Guid userId, CancellationToken ct);
    Task<bool> IsInUseAsync(Guid paymentMethodId, CancellationToken ct);
    Task DeleteAsync(PaymentMethod paymentMethod, CancellationToken ct);
}
