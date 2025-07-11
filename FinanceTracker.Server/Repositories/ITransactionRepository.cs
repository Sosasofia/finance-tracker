using FinanceTracker.Server.Models.Entities;

namespace FinanceTracker.Server.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> AddTransactionAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetTransactionsByUserAsync(Guid userId);
        Task<Transaction> GetTransactionByIdAsync(Guid id);
        Task<Transaction> GetTransactionsByIdAndUserAsync(Guid transactionId, Guid userId);
        Task DeleteTransactionAsync(Transaction transaction);
    }
}
