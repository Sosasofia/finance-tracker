using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Transactions.Queries.GetTransactionsList;

public class GetTransactionsQueryHandler
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionsQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<TransactionResponse>> Handle(GetTransactionsListQuery query, CancellationToken ct)
    {
        var transactions = await _transactionRepository.GetTransactionsByUserAsync(query.UserId);

        return transactions.Select(TransactionResponse.MapFrom);
    }
}
