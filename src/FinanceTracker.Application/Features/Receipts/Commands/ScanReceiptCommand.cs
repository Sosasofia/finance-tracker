using FinanceTracker.Application.DTOs;
using MediatR;

namespace FinanceTracker.Application.Features.Receipts.Commands;
public record ScanReceiptCommand(Stream FileStream, string UserId) : IRequest<ExtractedReceiptDto>;
