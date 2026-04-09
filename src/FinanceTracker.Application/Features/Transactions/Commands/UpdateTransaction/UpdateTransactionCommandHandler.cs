using AutoMapper;
using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Interfaces;
using FinanceTracker.Domain.ValueObjects;

namespace FinanceTracker.Application.Features.Transactions.Commands.UpdateTransaction;

public class UpdateTransactionCommandHandler
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public UpdateTransactionCommandHandler(ITransactionRepository repository, IMapper mapper)
    {
        _transactionRepository = repository;
        _mapper = mapper;
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

        transaction.AssignCategory(command.CategoryId);
        transaction.AssignPaymentMethod(command.PaymentMethodId);

        await _transactionRepository.UpdateTransactionAsync(transaction);

        return _mapper.Map<TransactionResponse>(transaction);
    }
}
