using FinanceTracker.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceTracker.Server.Controllers;

[Authorize]
[EnableRateLimiting("fixed")]
[Route("api/receipts")]
[ApiController]
public class ReceiptsController : ControllerBase
{
    private readonly IReceiptScannerService _receiptScannerService;

    public ReceiptsController(IReceiptScannerService receiptScannerService)
    {
        _receiptScannerService = receiptScannerService;
    }

    [HttpPost("scan")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ScanReceipt(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension))
        {
            return BadRequest("Invalid file type. Only JPG, PNG, and PDF are supported.");
        }

        using var stream = file.OpenReadStream();
        var result = await _receiptScannerService.ScanReceiptAsync(stream);

        return Ok(result);
    }
}
