
using FinanceTracker.Application.Features;
using FinanceTracker.Application.Features.Transactions;

namespace FinanceTracker.Application.Interfaces;

public interface ITransactionApplicationService
{
    Task<Response<TransactionResponse>> AddTransactionAsync(CreateTransaction transactionCreateDTO, Guid userID);
    Task<IEnumerable<TransactionResponse>> GetTransactionsByUserAsync(Guid userId);
    Task<Response<bool>> DeleteTransactionAsync(Guid transactionId, Guid userId);
    Task<TransactionResponse> GetTransactionByIdAndUserAsync(Guid transactionId, Guid userId);
    Task<TransactionResponse> RestoreDeleteTransactionAsync(Guid transactionId, Guid userId);
    Task<Response<TransactionResponse>> UpdateTransactionAsync(Guid transactionId, UpdateTransaction transactionCreateDTO, Guid userId);
}
