using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Transactions.Queries.GetTransactionById;

public class GetTransactionByIdQueryHandler
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionByIdQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<TransactionResponse> Handle(GetTransactionByIdQuery query, CancellationToken ct)
    {
        var transaction = await _transactionRepository.GetTransactionsByIdAndUserAsync(
            query.TransactionId,
            query.UserId);

        if (transaction == null)
        {
            throw new NotFoundException("Transaction", query.TransactionId);
        }

        return TransactionResponse.MapFrom(transaction);
    }
}
