using FinanceTracker.Application.Common.DTOs;
using FinanceTracker.Application.Features.Transactions;

namespace FinanceTracker.Application.Common.Interfaces.Services;

public interface ITransactionService
{
    /// <summary>
    /// Asynchronously adds a new transaction for the specified user.
    /// </summary>
    /// <param name="transactionCreateDto">The data transfer object containing the details of the transaction to be created.</param>
    /// <param name="userID">The unique identifier of the user for whom the transaction is being added.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a <see cref="Result{T}"/> object 
    /// with a <see cref="TransactionResponse"/> if the operation is successful, or an error if the operation fails.</returns>
    Task<Result<TransactionResponse>> AddTransactionAsync(CreateTransactionDto transactionCreateDto, Guid userID);
    Task<IEnumerable<TransactionResponse>> GetTransactionsByUserAsync(Guid userId);
    Task<Response<bool>> DeleteTransactionAsync(Guid transactionId, Guid userId);
    Task<TransactionResponse> GetTransactionByIdAndUserAsync(Guid transactionId, Guid userId);
    Task<TransactionResponse> RestoreDeleteTransactionAsync(Guid transactionId, Guid userId);
    Task<Response<TransactionResponse>> UpdateTransactionAsync(Guid transactionId, UpdateTransactionDto transactionCreateDto, Guid userId);
}
