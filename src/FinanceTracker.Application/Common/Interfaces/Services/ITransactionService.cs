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
    /// <summary>
    /// Retrieves a collection of transactions associated with the specified user.
    /// </summary>
    /// <remarks>This method performs an asynchronous operation to fetch transactions for the specified user. 
    /// Ensure that the <paramref name="userId"/> is valid and corresponds to an existing user.</remarks>
    /// <param name="userId">The unique identifier of the user whose transactions are to be retrieved.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains an enumerable collection  of <see
    /// cref="TransactionResponse"/> objects representing the user's transactions.</returns>
    Task<IEnumerable<TransactionResponse>> GetTransactionsByUserAsync(Guid userId);
    /// <summary>
    /// Deletes a transaction associated with the specified transaction ID and user ID.
    /// </summary>
    /// <param name="transactionId">The unique identifier of the transaction to delete.</param>
    /// <param name="userId">The unique identifier of the user requesting the deletion.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a  <see cref="Response{T}"/> object
    /// with a boolean value indicating whether the  transaction was successfully deleted.</returns>
    Task<bool> DeleteTransactionAsync(Guid transactionId, Guid userId);
    /// <summary>
    /// Retrieves a transaction associated with the specified transaction ID and user ID.
    /// </summary>
    /// <remarks>This method performs a lookup for a transaction based on the provided identifiers. Ensure
    /// that  both <paramref name="transactionId"/> and <paramref name="userId"/> are valid and non-empty.</remarks>
    /// <param name="transactionId">The unique identifier of the transaction to retrieve.</param>
    /// <param name="userId">The unique identifier of the user associated with the transaction.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a  <see cref="TransactionResponse"/>
    /// object representing the transaction details, or <c>null</c>  if no matching transaction is found.</returns>
    Task<TransactionResponse> GetTransactionByIdAndUserAsync(Guid transactionId, Guid userId);
    /// <summary>
    /// Restores a previously deleted transaction for the specified user.
    /// </summary>
    /// <remarks>This method reverts the deletion of a transaction, making it active again.  Ensure that the
    /// transaction ID and user ID are valid and correspond to an existing deleted transaction.</remarks>
    /// <param name="transactionId">The unique identifier of the transaction to restore.</param>
    /// <param name="userId">The unique identifier of the user associated with the transaction.</param>
    /// <returns>A <see cref="TransactionResponse"/> object containing details of the restored transaction.</returns>
    Task<TransactionResponse> RestoreDeleteTransactionAsync(Guid transactionId, Guid userId);
    /// <summary>
    /// Updates an existing transaction with the specified details.
    /// </summary>
    /// <param name="transactionId">The unique identifier of the transaction to update.</param>
    /// <param name="transactionCreateDto">An object containing the updated transaction details.</param>
    /// <param name="userId">The unique identifier of the user performing the update.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains a  <see cref="Response{T}"/> object
    /// with the updated transaction details.</returns>
    Task<Response<TransactionResponse>> UpdateTransactionAsync(Guid transactionId, UpdateTransactionDto transactionCreateDto, Guid userId);
}
