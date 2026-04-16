using MediatR;

namespace FinanceTracker.Application.Features.Transactions.Queries.ExportTransactions;

public record ExportTransactionsQuery(
    Guid UserId,
    DateTime Start,
    DateTime End,
    ExportFormat Format
) : IRequest<byte[]>;

public enum ExportFormat
{
    Excel,
    Csv
}
