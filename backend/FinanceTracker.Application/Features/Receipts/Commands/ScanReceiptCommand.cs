using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.DTOs;

namespace FinanceTracker.Application.Features.Receipts.Commands;

public record ScanReceiptCommand(Stream FileStream, string UserId) : IRequest<ExtractedReceiptDto>;
