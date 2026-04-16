using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> AddTransactionAsync(Transaction transaction, CancellationToken ct);
    Task<IEnumerable<Transaction>> GetTransactionsByUserAsync(Guid userId, CancellationToken ct);
    Task<Transaction> GetTransactionByIdAsync(Guid id, CancellationToken ct);
    Task<Transaction> GetTransactionsByIdAndUserAsync(Guid transactionId, Guid userId, CancellationToken ct);
    Task DeleteTransactionAsync(Transaction transaction, CancellationToken ct);
    Task<Transaction> RestoreDeleteTransactionAsync(Transaction transaction, CancellationToken ct);
    Task<Transaction> GetTransactionByIdAndUserIncludingDeletedAsync(Guid transactionId, Guid userId, CancellationToken ct);
    Task UpdateTransactionAsync(Transaction transaction, CancellationToken ct);
    Task<IEnumerable<Transaction>> GetByUserAndDateRangeAsync(Guid userId, DateTime startDate, DateTime endDate, CancellationToken ct);
}
