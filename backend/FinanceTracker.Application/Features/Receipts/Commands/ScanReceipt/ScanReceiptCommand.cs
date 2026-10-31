using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.DTOs;

namespace FinanceTracker.Application.Features.Receipts.Commands.ScanReceipt;

public record ScanReceiptCommand(
    Stream FileStream,
    long FileLength,
    string OriginalFileName,
    string ContentType,
    Guid UserId) : IRequest<ExtractedReceiptDto>;
