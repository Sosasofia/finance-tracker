using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Features.Transactions.Commands.DeleteTransaction;

public class DeleteTransactionCommandHandler : IRequestHandler<DeleteTransactionCommand>
{

    private readonly ITransactionRepository _transactionRepository;

    public DeleteTransactionCommandHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task Handle(DeleteTransactionCommand command, CancellationToken ct)
    {
        var transaction = await _transactionRepository.GetTransactionsByIdAndUserAsync(command.Id, command.UserId, ct);

        if (transaction == null) throw new UnauthorizedAccessException("Denied access.");

        transaction.SoftDelete();

        await _transactionRepository.UpdateTransactionAsync(transaction, ct);
    }
}
