using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Transactions.Queries.GetTransactionsList;

public class GetTransactionsQueryHandler : IRequestHandler<GetTransactionsQuery, IEnumerable<TransactionResponse>>
{
    private readonly ITransactionRepository _transactionRepository;

    public GetTransactionsQueryHandler(ITransactionRepository transactionRepository)
    {
        _transactionRepository = transactionRepository;
    }

    public async Task<IEnumerable<TransactionResponse>> Handle(GetTransactionsQuery query, CancellationToken ct)
    {
        var transactions = await _transactionRepository.GetTransactionsByUserAsync(query.UserId, ct);

        return transactions.Select(TransactionResponse.MapFrom);
    }
}
