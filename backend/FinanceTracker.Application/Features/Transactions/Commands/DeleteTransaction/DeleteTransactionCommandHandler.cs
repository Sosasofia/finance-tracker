using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Transactions.Commands.DeleteTransaction;

public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand, Unit>
{

    private readonly ITransactionRepository _transactionRepository;

    public DeleteTransactionCommandHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<Unit> Handle(DeleteTransactionCommand command, CancellationToken ct)
    {
        var transaction = await _transactionRepository.GetTransactionsByIdAndUserAsync(command.Id, command.UserId, ct);

        if (transaction == null)
        {
            throw new NotFoundException("Transaction to be deleted not found.");
        }

        transaction.SoftDelete();

        await _transactionRepository.UpdateTransactionAsync(transaction, ct);

        return Unit.Value;
    }
}
