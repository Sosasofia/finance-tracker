using FinanceTracker.Application.Common.Interfaces.Security;
using FinanceTracker.Application.Common.Interfaces.Services;
using FinanceTracker.Application.Features.Transactions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers;

[ApiController]
[Route("api/[controller]")]
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

        if (userId == null)
        {
            return Unauthorized("Missing or invalid user ID claim");
        }

        var result = await _transactionService.AddTransactionAsync(transaction, userId.Value);

        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(new { errors = result.Errors });
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetTransactionsByUser()
    {
        var userId = _currentUserService.UserId();

        if (userId == null)
        {
            return Unauthorized("Missing or invalid user ID claim");
        }

        var transactions = await _transactionService.GetTransactionsByUserAsync(userId.Value);

        if (!transactions.Any())
        {
            return NotFound("No transactions found for this user");
        }

        return Ok(transactions);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var userId = _currentUserService.UserId();

        if (userId == null)
        {
            return Unauthorized("Missing or invalid user ID claim");
        }

        var isDeleted = await _transactionService.DeleteTransactionAsync(id, userId.Value);

        return isDeleted ? NoContent() : BadRequest("Error deleting the transaction.");
    }

    [HttpPatch("{id}/restore")]
    public async Task<IActionResult> RestoreTransaction(Guid id)
    {
        var userId = _currentUserService.UserId();

        if (userId == null)
        {
            return Unauthorized("Missing or invalid user ID claim");
        }

        var restoredTransaction = await _transactionService.RestoreDeleteTransactionAsync(id, userId.Value);

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

        if (userId == null)
        {
            return Unauthorized("Missing or invalid user ID claim");
        }


        var updatedTransaction = await _transactionService.UpdateTransactionAsync(id, transactionUpdateDTO, userId.Value);

        if (updatedTransaction.Success == false)
        {
            return BadRequest(updatedTransaction.Message);
        }

        return Ok(updatedTransaction.Data);
    }

    [HttpGet("export/csv")]
    public async Task<IActionResult> ExportToCsv([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
    {
        var userId = _currentUserService.UserId();

        if (userId == null)
        {
            return Unauthorized("Missing or invalid user ID claim");
        }

        var fileBytes = await _transactionService.ExportTransactionsToCsv(userId.Value, startDate, endDate);

        return File(fileBytes,
            "text/csv",
            "transactions.csv");
    }

    [HttpGet("export/excel")]
    public async Task<IActionResult> ExportToExcel([FromQuery] DateTime start, [FromQuery] DateTime end)
    {
        var userId = _currentUserService.UserId();

        if (userId == null)
        {
            return Unauthorized("Missing or invalid user ID claim");
        }

        var fileBytes = await _transactionService.ExportTransactionsToExcel(userId.Value, start, end);

        return File(fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            "transactions.xlsx");
    }
}
