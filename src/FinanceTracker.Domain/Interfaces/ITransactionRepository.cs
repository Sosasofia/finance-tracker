using FinanceTracker.Domain.Entities;

namespace FinanceTracker.Domain.Interfaces;

public interface ITransactionRepository
{
    Task<Transaction> AddTransactionAsync(Transaction transaction);
    Task<IEnumerable<Transaction>> GetTransactionsByUserAsync(Guid userId);
    Task<Transaction> GetTransactionByIdAsync(Guid id);
    Task<Transaction> GetTransactionsByIdAndUserAsync(Guid transactionId, Guid userId);
    Task DeleteTransactionAsync(Transaction transaction);
    Task<Transaction> RestoreDeleteTransactionAsync(Transaction transaction);
    Task<Transaction> GetTransactionByIdAndUserIncludingDeletedAsync(Guid transactionId, Guid userId);
    Task UpdateTransactionAsync(Transaction transaction);
}
