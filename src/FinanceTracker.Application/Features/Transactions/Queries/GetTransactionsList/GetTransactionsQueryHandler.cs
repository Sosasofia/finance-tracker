using AutoMapper;
using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Entities;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Transactions.Queries.GetTransactionsList;

public class GetTransactionsQueryHandler
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public GetTransactionsQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<TransactionResponse>> Handle(GetTransactionsListQuery query, CancellationToken ct)
    {
        var transactions = await _transactionRepository.GetTransactionsByUserAsync(query.UserId);

        return _mapper.Map<IEnumerable<Transaction>, IEnumerable<TransactionResponse>>(transactions);
    }
}
