using System.Security.Claims;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceTracker.Server.Controllers;

[Authorize]
[EnableRateLimiting("receipt-scanner-policy")]
[Route("api/receipts")]
[ApiController]
public class ReceiptsController : ControllerBase
{
    private readonly IReceiptScannerService _receiptScannerService;
    private readonly IReceiptMappingService _receiptMappingService;

    public ReceiptsController(IReceiptScannerService receiptScannerService, IReceiptMappingService receiptMappingService)
    {
        _receiptScannerService = receiptScannerService;
        _receiptMappingService = receiptMappingService;
    }

    [HttpPost("scan")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ScanReceipt(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        const int MaxFileSize = 5 * 1024 * 1024;
        if (file.Length > MaxFileSize)
        {
            return BadRequest("File is too large. Please upload an image under 5MB.");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest("Invalid file type. Only JPG, PNG, and PDF are supported.");
        }

        using var stream = file.OpenReadStream();
        var rawAzureData = await _receiptScannerService.ScanReceiptAsync(stream);

        var mappedDto = await _receiptMappingService.MapAzureResultAsync(rawAzureData, userId);

        return Ok(mappedDto);
    }
}
