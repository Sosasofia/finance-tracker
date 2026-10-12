using System.Security.Claims;
using FinanceTracker.Application.Features.Receipts.Commands;
using MediatR;
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
    private readonly ISender _mediator;

    public ReceiptsController(ISender mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("scan")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ScanReceipt(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded.");

        const int MaxFileSize = 5 * 1024 * 1024;
        if (file.Length > MaxFileSize) return BadRequest("File is too large. Please upload an image under 5MB.");

        var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".pdf" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();

        if (!allowedExtensions.Contains(extension)) return BadRequest("Invalid file type. Only JPG, PNG, and PDF are supported.");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return Unauthorized();

        using var stream = file.OpenReadStream();
        
        var mappedDto = await _mediator.Send(new ScanReceiptCommand(stream, userId));

        return Ok(mappedDto);
    }
}
