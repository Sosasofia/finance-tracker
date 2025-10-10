using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.DTOs.Response;

namespace FinanceTracker.Server.Services
{
    public interface ITransactionService
    {
        Task<Response<TransactionResponse>> AddTransactionAsync(TransactionCreateDTO transactionCreateDTO, Guid userID);
        Task<IEnumerable<TransactionResponse>> GetTransactionsByUserAsync(Guid userId);
        Task<Response<bool>> DeleteTransactionAsync(Guid transactionId, Guid userId);
        Task<TransactionResponse> GetTransactionByIdAndUserAsync(Guid transactionId, Guid userId);
        Task<TransactionResponse> RestoreDeleteTransactionAsync(Guid transactionId, Guid userId);
        Task<Response<TransactionResponse>> UpdateTransactionAsync(Guid transactionId, TransactionUpdateDTO transactionCreateDTO, Guid userId);
    }
}
