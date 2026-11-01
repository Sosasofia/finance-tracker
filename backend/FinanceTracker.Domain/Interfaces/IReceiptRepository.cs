using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface IReceiptRepository
{
    Task AddAsync(Receipt receipt, CancellationToken ct);
    Task<Receipt?> GetByIdAndUserAsync(Guid receiptId, Guid userId, CancellationToken ct);
    Task<IReadOnlyList<Receipt>> GetByUserAsync(Guid userId, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
