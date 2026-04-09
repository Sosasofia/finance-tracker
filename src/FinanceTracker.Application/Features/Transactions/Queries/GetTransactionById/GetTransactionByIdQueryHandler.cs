using AutoMapper;
using FinanceTracker.Application.Common.Exceptions;
using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Transactions.Queries.GetTransactionById;

public class GetTransactionByIdQueryHandler
{
    private readonly ITransactionRepository _transactionRepository;
    private readonly IMapper _mapper;

    public GetTransactionByIdQueryHandler(ITransactionRepository transactionRepository, IMapper mapper)
    {
        _transactionRepository = transactionRepository;
        _mapper = mapper;
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

        return _mapper.Map<TransactionResponse>(transaction);
    }
}
