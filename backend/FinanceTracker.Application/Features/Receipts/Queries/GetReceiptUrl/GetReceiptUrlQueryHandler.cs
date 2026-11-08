using FinanceTracker.Application.Common.Interfaces;

namespace FinanceTracker.Application.Features.Receipts.Queries.GetReceiptUrl;

public sealed class GetReceiptUrlQueryHandler
    : IRequestHandler<GetReceiptUrlQuery, ReceiptSecureUrlResponse>
{
    private readonly IReceiptStorageService _receiptStorage;

    public GetReceiptUrlQueryHandler(IReceiptStorageService receiptStorage)
    {
        _receiptStorage = receiptStorage;
    }

    public async Task<ReceiptSecureUrlResponse> Handle(
        GetReceiptUrlQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _receiptStorage.GetSecureUrlAsync(
            request.ReceiptId,
            request.UserId,
            cancellationToken);

        return new ReceiptSecureUrlResponse(result.Url, result.ExpiresAt);
    }
}
