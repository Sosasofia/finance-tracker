using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.Response;
using FinanceTracker.Server.Models;

namespace FinanceTracker.Server.Services
{
    public interface ITransactionService
    {
        Task<Response<TransactionResponse>> AddTransactionAsync(TransactionCreateDTO transactionCreateDTO, Guid userID);
        Task<IEnumerable<TransactionResponse>> GetTransactionsByUserAsync(Guid userId);
    }
}
