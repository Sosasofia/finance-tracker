using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Interfaces;
using MediatR;

namespace FinanceTracker.Application.Features.Transactions.Queries.GetTransactionById;

public class GetTransactionByIdQueryHandler : IRequestHandler<GetTransactionByIdQuery, TransactionResponse>
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
            query.UserId,
            ct);

        if (transaction == null)
        {
            throw new NotFoundException("Transaction", query.TransactionId);
        }

        return TransactionResponse.MapFrom(transaction);
    }
}
