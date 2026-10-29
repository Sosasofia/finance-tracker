using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Common.Metrics;
using FinanceTracker.Application.DTOs;
using FinanceTracker.Domain.Interfaces;

namespace FinanceTracker.Application.Features.Receipts.Commands;

public class ScanReceiptCommandHandler : IRequestHandler<ScanReceiptCommand, ExtractedReceiptDto>
{
    private readonly IReceiptScannerService _receiptScannerService;
    private readonly IReceiptMappingService _receiptMappingService;

    public ScanReceiptCommandHandler(IReceiptScannerService receiptScannerService, IReceiptMappingService receiptMappingService)
    {
        _receiptScannerService = receiptScannerService;
        _receiptMappingService = receiptMappingService;
    }

    public async Task<ExtractedReceiptDto> Handle(ScanReceiptCommand request, CancellationToken cancellationToken)
    {
        var rawAzureData = await _receiptScannerService.ScanReceiptAsync(request.FileStream);
        var mappedDto = await _receiptMappingService.MapAzureResultAsync(rawAzureData, request.UserId);

        // Metric
        ApplicationMetrics.ReceiptsScannedCounter
            .Add(1, new KeyValuePair<string, object?>("status", "success"));

        if (mappedDto.Amount.HasValue)
        {
            ApplicationMetrics.ReceiptAmountHistogram
                .Record((double)mappedDto.Amount.Value, new KeyValuePair<string, object?>("status", "success"));
        }

        return mappedDto;
    }
}
