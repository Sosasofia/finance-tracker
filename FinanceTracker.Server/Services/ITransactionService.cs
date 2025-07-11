using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.DTOs.Response;
using FinanceTracker.Server.Models.Entities;

namespace FinanceTracker.Server.Services
{
    public interface ITransactionService
    {
        Task<Response<TransactionResponse>> AddTransactionAsync(TransactionCreateDTO transactionCreateDTO, Guid userID);
        Task<IEnumerable<TransactionResponse>> GetTransactionsByUserAsync(Guid userId);
        Task<Response<bool>> DeleteTransactionAsync(Guid transactionId, Guid userId);
        Task<TransactionResponse> GetTransactionByIdAndUserAsync(Guid transactionId, Guid userId);
    }
}
