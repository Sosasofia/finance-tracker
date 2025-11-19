using FinanceTracker.Application.Common.DTOs;
using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize]
[Produces("application/json")]
[Consumes("application/json")]
[ProducesResponseType(typeof(void), StatusCodes.Status401Unauthorized)]
[ProducesResponseType(typeof(void), StatusCodes.Status403Forbidden)]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ICurrentUserService _currentUserService;

    public TransactionController(ITransactionService transactionService, ICurrentUserService currentUserService)
    {
        _transactionService = transactionService;
        _currentUserService = currentUserService;
    }

    /// <summary>
    /// Creates a new transaction for the authenticated user.
    /// </summary>
    /// <remarks>
    /// <p><strong>Description:</strong>  
    /// Adds a new income or expense associated with the currently authenticated user.</p>
    ///
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>The transaction must include amount, date, categoryId, and paymentMethodId.</li>
    ///   <li>Validation is handled by the service layer.</li>
    ///   <li>If validation fails, detailed errors are returned.</li>
    /// </ul>
    ///
    /// <p><strong>Result:</strong>  
    /// Returns <c>201 Created</c> with the created transaction and a Location header.</p>
    /// </remarks>
    /// 
    /// <param name="transaction">Transaction details required for creation.</param>
    /// <response code="201">Transaction successfully created.</response>
    /// <response code="400">Invalid transaction data.</response>
    [HttpPost]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionResponse>> Create([FromBody] CreateTransactionDto transaction)
    {
        if (transaction == null)
        {
            return BadRequest("The transaction object cannot be null.");
        }

        var userId = _currentUserService.UserId();

        var result = await _transactionService.AddTransactionAsync(transaction, userId);

        if (!result.IsSuccess)
        {
            return BadRequest(new { errors = result.Errors });
        }

        return CreatedAtAction(
                nameof(GetTransactionById),
                new { id = result.Value.Id },
                result.Value);
    }

    /// <summary>
    /// Retrieves all transactions for the authenticated user.
    /// </summary>
    /// <remarks>
    /// <p><strong>Description:</strong>  
    /// Returns all transactions belonging to the user (including income and expenses).</p>
    ///
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>Results include category and payment method information when available.</li>
    ///   <li>If the user has no transactions, a 404 response is returned.</li>
    /// </ul>
    ///
    /// <p><strong>Result:</strong>  
    /// <c>200 OK</c> with an array of transactions.</p>
    /// </remarks>
    ///
    /// <response code="200">Transactions returned successfully.</response>
    /// <response code="404">No transactions found.</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TransactionResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetTransactionsByUser()
    {
        var userId = _currentUserService.UserId();

        var transactions = await _transactionService.GetTransactionsByUserAsync(userId);

        if (!transactions.Any())
        {
            return NotFound("No transactions found for this user");
        }

        return Ok(transactions);
    }

    /// <summary>
    /// Retrieves a specific transaction by id.
    /// </summary>
    /// <remarks>
    /// <p><strong>Description:</strong>  
    /// Fetches a single transaction that belongs to the authenticated user.</p>
    ///
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>If the transaction does not exist or does not belong to the user, a 404 is returned.</li>
    /// </ul>
    ///
    /// <p><strong>Result:</strong>  
    /// <c>200 OK</c> with the transaction.</p>
    /// </remarks>
    ///
    /// <param name="id">Transaction identifier.</param>
    /// <response code="200">Transaction returned.</response>
    /// <response code="404">Transaction not found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionResponse>> GetTransactionById(Guid id)
    {
        var userId = _currentUserService.UserId();

        var transaction = await _transactionService.GetTransactionByIdAndUserAsync(id, userId);

        return transaction == null ? NotFound($"Transaction with id: {id} not found") : Ok(transaction);
    }

    /// <summary>
    /// Deletes a transaction.
    /// </summary>
    /// <remarks>
    /// <p><strong>Description:</strong>  
    /// Deletes the specified transaction only if it belongs to the authenticated user.</p>
    ///
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>If the transaction is soft-deleted, it can later be restored.</li>
    ///   <li>If deletion fails, a 400 response is returned.</li>
    /// </ul>
    ///
    /// <p><strong>Result:</strong>  
    /// <c>204 No Content</c> when the delete operation is successful.</p>
    /// </remarks>
    ///
    /// <param name="id">Transaction identifier.</param>
    /// <response code="204">Transaction deleted successfully.</response>
    /// <response code="400">The transaction could not be deleted.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(void), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = _currentUserService.UserId();

        var isDeleted = await _transactionService.DeleteTransactionAsync(id, userId);

        return isDeleted ? NoContent() : BadRequest("Error deleting the transaction.");
    }

    /// <summary>
    /// Restores a soft-deleted transaction.
    /// </summary>
    /// <remarks>
    /// <p><strong>Description:</strong>  
    /// Restores a previously deleted transaction, if it belongs to the user.</p>
    ///
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>Only soft-deleted transactions can be restored.</li>
    ///   <li>If not found or restoration fails, a 400 is returned.</li>
    /// </ul>
    ///
    /// <p><strong>Result:</strong>  
    /// <c>200 OK</c> with the restored transaction.</p>
    /// </remarks>
    ///
    /// <param name="id">Transaction identifier.</param>
    /// <response code="200">Transaction restored.</response>
    /// <response code="400">Restoration failed.</response>
    [HttpPatch("{id}/restore")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RestoreTransaction(Guid id)
    {
        var userId = _currentUserService.UserId();

        var restoredTransaction = await _transactionService.RestoreDeleteTransactionAsync(id, userId);

        if (restoredTransaction == null)
        {
            return BadRequest("Error retrieving deleted transaction");
        }

        return Ok(restoredTransaction);
    }

    /// <summary>
    /// Updates an existing transaction.
    /// </summary>
    /// <remarks>
    /// <p><strong>Description:</strong>  
    /// Modifies fields on a transaction belonging to the authenticated user.</p>
    ///
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>If validation fails, details are provided in the response.</li>
    ///   <li>If the transaction does not belong to the user, the update fails.</li>
    /// </ul>
    ///
    /// <p><strong>Result:</strong>  
    /// <c>200 OK</c> with the updated transaction.</p>
    /// </remarks>
    ///
    /// <param name="id">Transaction ID.</param>
    /// <param name="transactionUpdateDTO">Updated transaction data.</param>
    /// <response code="200">Transaction updated.</response>
    /// <response code="400">Invalid update data.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TransactionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ErrorResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] UpdateTransactionDto transactionUpdateDTO)
    {
        if (transactionUpdateDTO == null)
        {
            return BadRequest("Transaction update data cannot be null");
        }

        var userId = _currentUserService.UserId();

        var updatedTransaction = await _transactionService.UpdateTransactionAsync(id, transactionUpdateDTO, userId);

        if (updatedTransaction.Success == false)
        {
            return BadRequest(updatedTransaction.Message);
        }

        return Ok(updatedTransaction.Data);
    }

    /// <summary>
    /// Exports the user's transactions as a CSV file.
    /// </summary>
    /// <remarks>
    /// <p><strong>Description:</strong>  
    /// Generates a CSV containing all user transactions within the specified date range.</p>
    ///
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>Date range is inclusive.</li>
    ///   <li>Useful for reporting, backups, and data analysis.</li>
    /// </ul>
    ///
    /// <p><strong>Result:</strong>  
    /// A downloadable <c>.csv</c> file.</p>
    /// </remarks>
    ///
    /// <response code="200">CSV file generated.</response>
    [HttpGet("export/csv")]
    public async Task<IActionResult> ExportToCsv([FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
    {
        var userId = _currentUserService.UserId();

        var fileBytes = await _transactionService.ExportTransactionsToCsv(userId, dateFrom, dateTo);

        return File(fileBytes,
            "text/csv",
            "transactions.csv");
    }

    /// <summary>
    /// Exports the user's transactions as an Excel file.
    /// </summary>
    /// <remarks>
    /// <p><strong>Description:</strong>  
    /// Generates an Excel spreadsheet (.xlsx) with the user's transactions within the selected date range.</p>
    ///
    /// <p><strong>Details:</strong></p>
    /// <ul>
    ///   <li>Includes formatting appropriate for spreadsheets.</li>
    ///   <li>Date range is inclusive.</li>
    /// </ul>
    ///
    /// <p><strong>Result:</strong>  
    /// A downloadable <c>.xlsx</c> file.</p>
    /// </remarks>
    ///
    /// <response code="200">Excel file generated.</response>
    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportToExcel([FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
    {
        var userId = _currentUserService.UserId();

        var fileBytes = await _transactionService.ExportTransactionsToExcel(userId, dateFrom, dateTo);

        return File(fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "transactions.xlsx");
    }
}
