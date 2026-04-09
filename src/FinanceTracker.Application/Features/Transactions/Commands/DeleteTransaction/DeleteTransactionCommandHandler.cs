using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Transactions.Commands.DeleteTransaction;

public class DeleteTransactionCommandHandler
{

    private readonly ITransactionRepository _transactionRepository;

    public DeleteTransactionCommandHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task Handle(DeleteTransactionCommand command, CancellationToken ct)
    {
        var transaction = await _transactionRepository.GetTransactionsByIdAndUserAsync(command.Id, command.UserId);

        if (transaction == null) throw new UnauthorizedAccessException("Denied access.");

        transaction.SoftDelete();

        await _transactionRepository.UpdateTransactionAsync(transaction);
    }
}
