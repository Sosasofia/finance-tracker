using FinanceTracker.Application.Common.Interfaces;

namespace FinanceTracker.Application.Features.Receipts.Queries.GetReceiptUrl;

public sealed record GetReceiptUrlQuery(string ReceiptId, Guid UserId)
    : IRequest<ReceiptSecureUrlResponse>;

public sealed record ReceiptSecureUrlResponse(
    string Url,
    DateTimeOffset ExpiresAt);
