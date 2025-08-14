using FinanceTracker.Server.Models.DTOs;
using FinanceTracker.Server.Models.DTOs.Response;
using FinanceTracker.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinanceTracker.Server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = "CustomJWT")]
    public class TransactionController : BaseController
    {
        private readonly ITransactionService _transactionService;

        public TransactionController(ITransactionService transactionService)
        {
            _transactionService = transactionService;
        }

        [HttpPost]
        public async Task<ActionResult<TransactionResponse>> Create([FromBody] TransactionCreateDTO transaction)
        {
            if (transaction == null)
            {
                return BadRequest("Transaction cannot be null");
            }

            if (!UserId(out var userGuid))
            {
                return Unauthorized("Missing or invalid user ID claim");
            }

            var result = await _transactionService.AddTransactionAsync(transaction, userGuid);

            return result.Success ? Ok(result.Data) : BadRequest(result.Message);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetTransactionsByUser()
        {
            if (!UserId(out var userGuid))
            {
                return Unauthorized("Missing or invalid user ID claim");
            }

            var transactions = await _transactionService.GetTransactionsByUserAsync(userGuid);

            if (!transactions.Any())
            {
                return NotFound("No transactions found for this user");
            }

            return Ok(transactions);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            if (!UserId(out var userGuid))
            {
                return Unauthorized("Missing or invalid user ID claim");
            }

            try
            {
                await _transactionService.DeleteTransactionAsync(id, userGuid);
                return NoContent();
            }
            catch(UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest();
            }
        }

        [HttpPatch("{id}/restore")]
        public async Task<IActionResult> RestoreTransaction(Guid id)
        {
            if (!UserId(out var userGuid))
            {
                return Unauthorized("Missing or invalid user ID claim.");
            }

            try
            {
                var restoredTransaction = await _transactionService.RestoreDeleteTransactionAsync(id, userGuid);

                return Ok(restoredTransaction);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTransaction(Guid id, [FromBody] TransactionUpdateDTO transactionUpdateDTO)
        {
            if (transactionUpdateDTO == null)
            {
                return BadRequest("Transaction update data cannot be null");
            }
            if (!UserId(out var userGuid))
            {
                return Unauthorized("Missing or invalid user ID claim");
            }

            try
            {
                var updatedTransaction = await _transactionService.UpdateTransactionAsync(id, transactionUpdateDTO, userGuid);

                if(updatedTransaction.Success == false)
                {
                    return BadRequest(updatedTransaction.Message);
                }

                return Ok(updatedTransaction.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
