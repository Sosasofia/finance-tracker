using FinanceTracker.Application.Common.DTOs;
using FinanceTracker.Application.Features.Transactions;

namespace FinanceTracker.Application.Common.Interfaces.Services;

public interface ITransactionService
{
    Task<Response<TransactionResponse>> AddTransactionAsync(CreateTransactionDto transactionCreateDto, Guid userID);
    Task<IEnumerable<TransactionResponse>> GetTransactionsByUserAsync(Guid userId);
    Task<Response<bool>> DeleteTransactionAsync(Guid transactionId, Guid userId);
    Task<TransactionResponse> GetTransactionByIdAndUserAsync(Guid transactionId, Guid userId);
    Task<TransactionResponse> RestoreDeleteTransactionAsync(Guid transactionId, Guid userId);
    Task<Response<TransactionResponse>> UpdateTransactionAsync(Guid transactionId, UpdateTransactionDto transactionCreateDto, Guid userId);
}
