using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.Entities;

namespace FinanceTracker.Server.Repositories
{
    public interface ITransactionRepository
    {
        Task<Transaction> AddTransactionAsync(Transaction transaction);
        Task<IEnumerable<Transaction>> GetTransactionsByUserAsync(Guid userId);
        Task<Transaction> GetByIdAsync(Guid id);
        Task DeleteTransactionAsync(Guid id);
    }
}
