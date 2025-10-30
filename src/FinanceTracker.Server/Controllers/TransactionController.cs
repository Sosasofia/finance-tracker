using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize]
public class TransactionController : ControllerBase
{
    private readonly ITransactionService _transactionService;
    private readonly ICurrentUserService _currentUserService;

    public TransactionController(ITransactionService transactionService, ICurrentUserService currentUserService)
    {
        _transactionService = transactionService;
        _currentUserService = currentUserService;
    }

    [HttpPost]
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
            BadRequest(new { errors = result.Errors });
        }

        return CreatedAtAction(
                nameof(GetTransactionById),
                new { id = result.Value.Id },
                result.Value);
    }

    [HttpGet]
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

    [HttpGet("{id}")]
    public async Task<ActionResult<TransactionResponse>> GetTransactionById(Guid id)
    {
        var userId = _currentUserService.UserId();

        var transaction = await _transactionService.GetTransactionByIdAndUserAsync(userId, id);

        return transaction == null ? NotFound($"Transaction with id: {id} not found") : Ok(transaction);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = _currentUserService.UserId();

        var isDeleted = await _transactionService.DeleteTransactionAsync(id, userId);

        return isDeleted ? NoContent() : BadRequest("Error deleting the transaction.");
    }

    [HttpPatch("{id}/restore")]
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

    [HttpPut("{id}")]
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

    [HttpGet("export/csv")]
    public async Task<IActionResult> ExportToCsv([FromQuery] DateTime dateFrom, [FromQuery] DateTime dateTo)
    {
        var userId = _currentUserService.UserId();

        var fileBytes = await _transactionService.ExportTransactionsToCsv(userId, dateFrom, dateTo);

        return File(fileBytes,
            "text/csv",
            "transactions.csv");
    }

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
