using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Features.Transactions.Commands.CreateTransaction;
using FinanceTracker.Application.Features.Transactions.Commands.DeleteTransaction;
using FinanceTracker.Application.Features.Transactions.Commands.RestoreTransaction;
using FinanceTracker.Application.Features.Transactions.Commands.UpdateTransaction;
using FinanceTracker.Application.Features.Transactions.Models;
using FinanceTracker.Application.Features.Transactions.Queries.ExportTransactions;
using FinanceTracker.Application.Features.Transactions.Queries.GetTransactionById;
using FinanceTracker.Application.Features.Transactions.Queries.GetTransactionsList;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Route("api/transactions")]
[EnableRateLimiting("fixed")]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly ICurrentUserService _currentUserService;
    private readonly ISender _mediator;

    public TransactionController(
        ICurrentUserService currentUserService,
        ISender mediator)
    {
        _currentUserService = currentUserService;
        _mediator = mediator;
    }

    /// <summary>
    /// Creates a new transaction using the specified command and returns the unique identifier of the created
    /// transaction.
    /// </summary>
    /// <param name="command">The command containing the details required to create the transaction. Cannot be null.</param>
    /// <returns>An ActionResult containing the unique identifier of the newly created transaction. Returns a 201 Created
    /// response with the location of the new resource.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TransactionResponse>> Create([FromBody] CreateTransactionCommand command, CancellationToken ct)
    {
        var commandWithUser = command with { UserId = _currentUserService.UserId() };
        var response = await _mediator.Send(commandWithUser, ct);

        return CreatedAtAction(
            nameof(GetById),
            new { id = response.Id },
            response);
    }

    /// <summary>
    /// Retrieves all transactions associated with the currently authenticated user.
    /// </summary>
    /// <returns>An <see cref="ActionResult{T}"/> containing a collection of <see cref="TransactionResponse"/> objects for the
    /// current user. Returns a 404 Not Found response if no transactions are found.</returns>
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetAllByUser(CancellationToken ct)
    {
        var query = new GetTransactionsQuery(_currentUserService.UserId());
        var transactions = await _mediator.Send(query, ct);

        return Ok(transactions);
    }

    /// <summary>
    /// Retrieves the details of a specific transaction by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the transaction to retrieve.</param>
    /// <returns>An ActionResult containing the transaction details if found; otherwise, a NotFound result.</returns>
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TransactionResponse>> GetById(Guid id, CancellationToken ct)
    {
        var query = new GetTransactionByIdQuery(id, _currentUserService.UserId());
        var transaction = await _mediator.Send(query, ct);

        return Ok(transaction);
    }

    /// <summary>
    /// Deletes the transaction with the specified unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the transaction to delete.</param>
    /// <returns>A result indicating that the transaction was successfully deleted. Returns a 204 No Content response if the
    /// deletion is successful.</returns>
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var command = new DeleteTransactionCommand(id, _currentUserService.UserId());
        await _mediator.Send(command, ct);

        return NoContent();
    }

    /// <summary>
    /// Restores a previously deleted transaction identified by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the transaction to restore.</param>
    /// <returns>An ActionResult containing the restored transaction details if the operation is successful; otherwise, an
    /// appropriate error response.</returns>
    [HttpPatch("{id:guid}/restore")]
    public async Task<ActionResult<TransactionResponse>> RestoreTransaction(Guid id, CancellationToken ct)
    {
        var command = new RestoreTransactionCommand(id, _currentUserService.UserId());
        var restored = await _mediator.Send(command, ct);

        return Ok(restored);
    }

    /// <summary>
    /// Updates an existing transaction with the specified values.
    /// </summary>
    /// <param name="id">The unique identifier of the transaction to update.</param>
    /// <param name="command">An object containing the updated transaction details. The request body must include all required fields for the
    /// update.</param>
    /// <returns>An ActionResult containing the updated transaction details if the update is successful; otherwise, an
    /// appropriate error response.</returns>
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TransactionResponse>> UpdateTransaction(Guid id, [FromBody] UpdateTransactionCommand command, CancellationToken ct)
    {
        var commandWithUser = command with { Id = id, UserId = _currentUserService.UserId() };
        var updated = await _mediator.Send(commandWithUser, ct);

        return Ok(updated);
    }

    /// <summary>
    /// Exports transaction data within the specified date range as a CSV file for download.
    /// </summary>
    /// <remarks>The exported CSV includes all transactions for the current user within the specified date
    /// range. The method is intended for use via an HTTP GET request and returns the file as a downloadable
    /// response.</remarks>
    /// <param name="dateFrom">The start date of the transaction range to export. Only transactions on or after this date are included.</param>
    /// <param name="dateTo">The end date of the transaction range to export. Only transactions on or before this date are included.</param>
    /// <returns>A file result containing the exported transactions in CSV format. The file is named 'transactions.csv'.</returns>
    [HttpGet("export/csv")]
    public async Task<IActionResult> ExportToCsv([FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo, CancellationToken ct)
    {
        var query = new ExportTransactionsQuery(_currentUserService.UserId(), dateFrom, dateTo, ExportFormat.Csv);
        var fileBytes = await _mediator.Send(query, ct);

        return File(fileBytes, "text/csv", $"transactions_{DateTime.Now:yyyyMMdd}.csv");
    }

    /// <summary>
    /// Exports transaction data within the specified date range to an Excel file and returns it as a downloadable
    /// response.
    /// </summary>
    /// <remarks>The exported file is in the Open XML Spreadsheet format (.xlsx) and includes all transactions
    /// for the current user within the specified date range.</remarks>
    /// <param name="dateFrom">The start date of the transaction data to export. Only transactions on or after this date are included.</param>
    /// <param name="dateTo">The end date of the transaction data to export. Only transactions on or before this date are included.</param>
    /// <returns>A file result containing the exported transactions in Excel format. The file is named 'transactions.xlsx'.</returns>
    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportToExcel([FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo, CancellationToken ct)
    {
        var query = new ExportTransactionsQuery(_currentUserService.UserId(), dateFrom, dateTo, ExportFormat.Excel);
        var fileBytes = await _mediator.Send(query, ct);

        return File(fileBytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"transactions_{DateTime.Now:yyyyMMdd}.xlsx");
    }
}
