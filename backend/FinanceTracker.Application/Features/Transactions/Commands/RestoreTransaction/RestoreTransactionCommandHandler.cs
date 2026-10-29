using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Exceptions;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Transactions.Commands.RestoreTransaction;

public class RestoreTransactionCommandHandler : IRequestHandler<RestoreTransactionCommand, TransactionResponse>
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
            command.UserId,
            ct);

        if (transaction == null)
        {
            throw new NotFoundException("Transaction not found");
        }

        if (!transaction.IsDeleted)
        {
            throw new DomainException("Transaction is not deleted.");
        }

        var restored = await _transactionRepository.RestoreDeleteTransactionAsync(transaction, ct);

        return TransactionResponse.MapFrom(restored);
    }
}
