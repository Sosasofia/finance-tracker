using FinanceTracker.Application.Common.Interfaces;
using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Features.Receipts.Commands.UploadReceipt;
using FinanceTracker.Application.Features.Receipts.Commands.DeleteReceipt;
using FinanceTracker.Application.Features.Receipts.Commands.ScanReceipt;
using FinanceTracker.Application.Features.Receipts.Queries.DownloadReceipt;
using FinanceTracker.Application.Features.Receipts.Queries.GetReceiptUrl;
using FinanceTracker.Application.Features.Receipts.Queries.ListReceipts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Authorize]
[EnableRateLimiting("receipt-scanner-policy")]
[Route("api/receipts")]
public class ReceiptsController : ControllerBase
{
    private readonly IRequestDispatcher _dispatcher;
    private readonly ICurrentUserService _currentUserService;

    public ReceiptsController(IRequestDispatcher dispatcher,  ICurrentUserService currentUserService)
    {
        _dispatcher = dispatcher;
        _currentUserService = currentUserService;
    }

    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ReceiptStorageResult), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UploadReceipt(
        IFormFile file,
        CancellationToken ct)
    {
        await using var stream = file?.OpenReadStream() ?? Stream.Null;
        var result = await _dispatcher.Send(
            new UploadReceiptCommand(
                stream,
                file?.FileName ?? string.Empty,
                file?.ContentType ?? string.Empty,
                file?.Length ?? 0,
                Request.Form["displayName"].ToString(),
                _currentUserService.UserId()),
            ct);

        return CreatedAtAction(nameof(DownloadReceipt), new { receiptId = result.ReceiptId }, result);
    }

    [HttpGet("{receiptId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadReceipt(
        string receiptId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.Send(
            new DownloadReceiptQuery(receiptId, _currentUserService.UserId()),
            cancellationToken);

        return File(result.Content, result.ContentType, result.FileName, enableRangeProcessing: true);
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<ReceiptListItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ListReceipts(CancellationToken cancellationToken)
    {
        var result = await _dispatcher.Send(
            new ListReceiptsQuery(_currentUserService.UserId()),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("{receiptId}/process")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ProcessReceipt(
        string receiptId,
        CancellationToken cancellationToken)
    {
        var download = await _dispatcher.Send(
            new DownloadReceiptQuery(receiptId, _currentUserService.UserId()),
            cancellationToken);
        await using var content = download.Content;
        var result = await _dispatcher.Send(
            new ScanReceiptCommand(
                content,
                download.Length ?? 0,
                download.FileName,
                download.ContentType,
                _currentUserService.UserId()),
            cancellationToken);
        return Ok(result);
    }

    [HttpGet("{receiptId}/url")]
    [ProducesResponseType(typeof(ReceiptSecureUrlResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetReceiptUrl(
        string receiptId,
        CancellationToken cancellationToken)
    {
        var result = await _dispatcher.Send(
            new GetReceiptUrlQuery(receiptId, _currentUserService.UserId()),
            cancellationToken);

        return Ok(result);
    }

    [HttpDelete("{receiptId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteReceipt(
        string receiptId,
        CancellationToken cancellationToken)
    {
        await _dispatcher.Send(
            new DeleteReceiptCommand(receiptId, _currentUserService.UserId()),
            cancellationToken);

        return NoContent();
    }

    [HttpPost("scan")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> ScanReceipt(
        IFormFile file,
        CancellationToken cancellationToken)
    {
        var stream = file?.OpenReadStream() ?? Stream.Null;

        if (file == null || file.Length == 0)
        {
            return BadRequest("No file uploaded.");
        }

        var command = new ScanReceiptCommand(
            stream,
            file.Length,
            file.FileName,
            file.ContentType,
            _currentUserService.UserId());

        var mappedDto = await _dispatcher.Send(command, cancellationToken);

        return Ok(mappedDto);
    }
}
