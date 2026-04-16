using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Domain.ValueObjects;
using MediatR;

namespace FinanceTracker.Application.Features.Transactions.Commands.UpdateTransaction;

public class UpdateTransactionCommandHandler : IRequestHandler<UpdateTransactionCommand, TransactionResponse>
{
    private readonly ITransactionRepository _transactionRepository;

    public UpdateTransactionCommandHandler(ITransactionRepository repository)
    {
        _transactionRepository = repository;
    }

    public async Task<TransactionResponse> Handle(UpdateTransactionCommand command, CancellationToken ct)
    {
        var transaction = await _transactionRepository.GetTransactionsByIdAndUserAsync(command.Id, command.UserId, ct);

        if (transaction == null)
        {
            throw new NotFoundException($"Transaction {command.Id} was not found.");
        }

        transaction.UpdateDetails(command.Name, command.Description);
        transaction.ChangeAmount(Money.Create(command.Amount));
        transaction.ChangeDate(command.Date);

        if (command.CategoryId.HasValue)
        {
            transaction.AssignCategory(command.CategoryId.Value);
        }
        else
        {
            transaction.RemoveCategory();
        }

        if (command.PaymentMethodId.HasValue)
        {
            transaction.AssignPaymentMethod(command.PaymentMethodId.Value);
        }
        else
        {
            transaction.RemovePaymentMethod();
        }


        await _transactionRepository.UpdateTransactionAsync(transaction, ct);

        return TransactionResponse.MapFrom(transaction);
    }
}
