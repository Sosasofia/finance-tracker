using FinanceTracker.Application.Common.Interfaces.Security;
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
        public async Task<ActionResult<TransactionResponse>> Create([FromBody] TransactionCreateDTO transaction)
        {
            if (transaction == null)
            {
                return BadRequest("Transaction cannot be null");
            }

            var userId = _currentUserService.UserId();

            if (userId == null)
            {
                return Unauthorized("Missing or invalid user ID claim");
            }

            try
            {
                var result = await _transactionService.AddTransactionAsync(transaction, userId.Value);

                if (result.Success == false)
                {
                    return BadRequest(result.Message);
                }

                return Ok(result.Data);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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

            try
            {
                await _transactionService.DeleteTransactionAsync(id, userId.Value);
                return NoContent();
            }
            catch (UnauthorizedAccessException ex)
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
            var userId = _currentUserService.UserId();

            if (userId == null)
            {
                return Unauthorized("Missing or invalid user ID claim");
            }

            try
            {
                var restoredTransaction = await _transactionService.RestoreDeleteTransactionAsync(id, userId.Value);

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

            var userId = _currentUserService.UserId();

            if (userId == null)
            {
                return Unauthorized("Missing or invalid user ID claim");
            }

            try
            {
                var updatedTransaction = await _transactionService.UpdateTransactionAsync(id, transactionUpdateDTO, userId.Value);

                if (updatedTransaction.Success == false)
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
