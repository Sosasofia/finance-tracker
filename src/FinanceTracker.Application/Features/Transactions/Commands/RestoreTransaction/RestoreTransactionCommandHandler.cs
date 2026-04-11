using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Transactions.Commands.RestoreTransaction;

public class RestoreTransactionCommandHandler
{
    private readonly ITransactionRepository _transactionRepository;

    public RestoreTransactionCommandHandler(ITransactionRepository repository)
    {
        _transactionRepository = repository;
    }

    public async Task<TransactionResponse> Handle(RestoreTransactionCommand command, CancellationToken ct)
    {
        var transaction = await _transactionRepository.GetTransactionByIdAndUserIncludingDeletedAsync(
            command.TransactionId,
            command.UserId);

        if (transaction == null)
        {
            throw new UnauthorizedAccessException("Transaction not found or denied access.");
        }

        if (!transaction.IsDeleted)
        {
            throw new InvalidOperationException("Transaction is not deleted.");
        }

        var restored = await _transactionRepository.RestoreDeleteTransactionAsync(transaction);

        return TransactionResponse.MapFrom(restored);
    }
}
