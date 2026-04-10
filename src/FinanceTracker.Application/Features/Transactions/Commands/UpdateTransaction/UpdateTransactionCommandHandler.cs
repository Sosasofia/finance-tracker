using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Domain.ValueObjects;

namespace FinanceTracker.Application.Features.Transactions.Commands.UpdateTransaction;

public class UpdateTransactionCommandHandler
{
    private readonly ITransactionRepository _transactionRepository;

    public UpdateTransactionCommandHandler(ITransactionRepository repository)
    {
        _transactionRepository = repository;
    }

    public async Task<TransactionResponse> Handle(UpdateTransactionCommand command, CancellationToken ct)
    {
        var transaction = await _transactionRepository.GetTransactionsByIdAndUserAsync(command.Id, command.UserId);

        if (transaction == null)
        {
            throw new UnauthorizedAccessException("Transaction not found or denied access.");
        }

        transaction.UpdateDetails(command.Name, command.Description);

        transaction.ChangeAmount(Money.Create(command.Amount));
        transaction.ChangeDate(command.Date);

        await _transactionRepository.UpdateTransactionAsync(transaction);

        return TransactionResponse.MapFrom(transaction);
    }
}
